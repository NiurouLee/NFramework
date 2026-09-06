using System.Collections.Generic;
using System.IO;
using HybridCLR.Editor.Commands;
using UnityEditor;
using UnityEngine;

namespace Game.Editor.Tools
{
    /// <summary>
    /// HybridCLR 一键流程工具。
    ///
    /// 按 HybridCLR/Generate/All 的内部顺序拆成单步菜单，每步都标注了“为什么要做”；
    /// 最后提供“把生成的热更 DLL 和 AOT 元数据 DLL 拷贝进工程（.dll.bytes）”的方法，
    /// 拷贝完即可被 YooAsset 收集（JITDLL / AOTDLL 目录），运行时由 HybridClrService 加载。
    ///
    /// 使用建议：
    ///   - 日常只改热更代码：菜单 1（编译热更DLL）+ 菜单 7（拷贝DLL）即可；
    ///   - 首次配置 / 改了 AOT 侧 / 新增 AOT 泛型用法：跑“完整流程”（GenerateAll + 拷贝DLL）。
    /// </summary>
    public static class HybridCLRTool
    {
        // ==================== 路径与程序集名配置 ====================

        /// <summary>项目根目录（Assets 的上一级）</summary>
        private static string ProjectRoot => Path.GetDirectoryName(Application.dataPath);

        /// <summary>热更 DLL 源目录（第 1 步 CompileDll 的产物，全量脚本编译中转目录）</summary>
        private const string HotUpdateDllSrcRoot = "HybridCLRData/HotUpdateDlls";

        /// <summary>剥离后 AOT DLL 源目录（第 4 步 AOTDlls 的产物，也是运行时补充元数据的来源）</summary>
        private const string AOTDllSrcRoot = "HybridCLRData/AssembliesPostIl2CppStrip";

        /// <summary>工程内热更 DLL 目标目录（YooAsset 收集器 tag: JITDLL）</summary>
        private const string HotUpdateDllDestDir = "Assets/PackageAssets/JITDLL";

        /// <summary>工程内 AOT 元数据 DLL 目标目录（YooAsset 收集器 tag: AOTDLL）</summary>
        private const string AOTDllDestDir = "Assets/PackageAssets/AOTDLL";

        /// <summary>
        /// 热更程序集名（必须和 HybridCLR 设置里 hotUpdateAssemblyDefinitions 的 asmdef 名一致）。
        /// 运行时 LoadJIT 会按 YooAsset 的 JITDLL 标签逐个 Assembly.Load，这里只拷真正热更的程序集。
        /// </summary>
        private static readonly string[] HotUpdateAssemblyNames = { "Framework.Hot", "Game.Logic" };

        /// <summary>
        /// 需要补充元数据的 AOT 程序集名（对应运行时 LoadMetadataForAOTAssembly 加载的那批）。
        /// 热更代码用到了其泛型/元数据的 AOT 程序集都要列进来，按需增删；
        /// 源文件来自第 4 步的剥离后 AOT dll，不能用裁剪前的原始 dll。
        /// </summary>
        private static readonly string[] AOTAssemblyNames = { "mscorlib", "System", "System.Core", "UniTask" };

        // ==================== 完整流程 ====================

        /// <summary>一键跑完 GenerateAll 并把 DLL 拷进工程（推荐发布前使用）</summary>
        [MenuItem("FrameWorkTool/HybridCLRTool/完整流程 (GenerateAll + 拷贝DLL)", priority = 0)]
        public static void RunAllAndCopy()
        {
            GenerateAll();
            CopyHotUpdateAndAOTDlls();
        }

        /// <summary>一键执行 HybridCLR 全部生成步骤（等价于 HybridCLR/Generate/All）</summary>
        [MenuItem("FrameWorkTool/HybridCLRTool/0. 一键 GenerateAll", priority = 1)]
        public static void GenerateAll()
        {
            // GenerateAll 内部已经按 1→6 的正确顺序执行，前一步产物是后一步输入，不能乱序。
            PrebuildCommand.GenerateAll();
            Debug.Log("[HybridCLRTool] GenerateAll 完成");
        }

        // ==================== 单步执行 ====================

        /// <summary>
        /// 第 1 步：编译热更 DLL（CompileDll）。
        /// 为什么：把热更程序集（Framework.Hot / Game.Logic）编译成托管 DLL 输出到
        /// HybridCLRData/HotUpdateDlls/{平台}。注意它是“整个 Player 脚本全量编译”的中转目录，
        /// 里面那一堆 DLL 里真正热更的只有设置里配的那几个，其余是 AOT 分析用的副产物。
        /// 日常只改热更代码时，这一步 + 拷贝 DLL 就够。
        /// </summary>
        [MenuItem("FrameWorkTool/HybridCLRTool/1. 编译热更DLL (CompileDll)", priority = 10)]
        public static void Step1_CompileHotUpdateDlls()
        {
            CompileDllCommand.CompileDll(EditorUserBuildSettings.activeBuildTarget, EditorUserBuildSettings.development);
            Debug.Log("[HybridCLRTool] 1. 热更DLL编译完成");
        }

        /// <summary>
        /// 第 2 步：生成 Il2CppDef。
        /// 为什么：向本地 il2cpp 源码树注入 UnityVersion.h（native 代码按 Unity 版本走编译分支）
        /// 和 AssemblyManifest.cpp（native 层声明哪些程序集是热更的，运行时才能正确路由到解释器）。
        /// 只有改了热更程序集配置（增删 asmdef）才需要重跑。
        /// </summary>
        [MenuItem("FrameWorkTool/HybridCLRTool/2. 生成Il2CppDef", priority = 20)]
        public static void Step2_GenerateIl2CppDef()
        {
            Il2CppDefGeneratorCommand.GenerateIl2CppDef();
            Debug.Log("[HybridCLRTool] 2. Il2CppDef 生成完成");
        }

        /// <summary>
        /// 第 3 步：生成 link.xml（依赖第 1 步的热更 DLL）。
        /// 为什么：IL2CPP 构建主包时会裁剪托管代码，但热更 dll 是运行时才加载的，编译器看不到热更代码
        /// 引用了主包哪些类型。这一步扫描热更程序集引用的所有类型，写进 link.xml 标记 preserve，
        /// 防止这些 AOT 类型/成员被裁掉（漏了会 TypeLoadException/MissingMethodException）。
        /// </summary>
        [MenuItem("FrameWorkTool/HybridCLRTool/3. 生成link.xml (防裁剪)", priority = 30)]
        public static void Step3_GenerateLinkXml()
        {
            LinkGeneratorCommand.GenerateLinkXml(EditorUserBuildSettings.activeBuildTarget);
            Debug.Log("[HybridCLRTool] 3. link.xml 生成完成");
        }

        /// <summary>
        /// 第 4 步：生成剥离后的 AOT DLL（AOTDlls，依赖 IL2CPP，内部跑一次 buildScriptsOnly 构建）。
        /// 为什么：AOT 程序集经 IL2CPP 处理后会被裁剪，HybridCLR 需要“与主包最终状态一致”的剥离版
        /// 作为运行时补充元数据的来源，也给第 5、6 步分析用。所以必须切 IL2CPP 且脚本能编译通过。
        /// </summary>
        [MenuItem("FrameWorkTool/HybridCLRTool/4. 生成剥离AOTDLL", priority = 40)]
        public static void Step4_GenerateStripedAOTDlls()
        {
            StripAOTDllCommand.GenerateStripedAOTDlls(EditorUserBuildSettings.activeBuildTarget);
            Debug.Log("[HybridCLRTool] 4. 剥离AOTDLL 生成完成");
        }

        /// <summary>
        /// 第 5 步：生成 MethodBridge（依赖第 4 步的 AOT DLL）。
        /// 为什么：热更（解释执行）代码调用 AOT 泛型方法 / PInvoke / calli / 反 PInvoke 回调时，
        /// 需要 C++ 桥接函数把解释器调用转接到原生实现。扫描 AOT 与热更程序集生成 MethodBridge.cpp，
        /// 注入本地 il2cpp 随主包编译。没有桥，热更代码一调 AOT 泛型方法就崩。
        /// </summary>
        [MenuItem("FrameWorkTool/HybridCLRTool/5. 生成MethodBridge", priority = 50)]
        public static void Step5_GenerateMethodBridge()
        {
            MethodBridgeGeneratorCommand.GenerateMethodBridgeAndReversePInvokeWrapper(EditorUserBuildSettings.activeBuildTarget);
            Debug.Log("[HybridCLRTool] 5. MethodBridge 生成完成");
        }

        /// <summary>
        /// 第 6 步：生成 AOTGenericReferences（依赖第 1 步 + 第 4 步）。
        /// 为什么：提取热更代码用到的 AOT 泛型类型/泛型方法引用，写入 AOTGenericReferences.cs。
        /// IL2CPP 对泛型实例化裁剪最狠，运行时靠这份清单 + 补充元数据恢复，否则泛型相关调用会失败。
        /// </summary>
        [MenuItem("FrameWorkTool/HybridCLRTool/6. 生成AOTGenericReference", priority = 60)]
        public static void Step6_GenerateAOTGenericReference()
        {
            AOTReferenceGeneratorCommand.GenerateAOTGenericReference(EditorUserBuildSettings.activeBuildTarget);
            Debug.Log("[HybridCLRTool] 6. AOTGenericReference 生成完成");
        }

        // ==================== 拷贝 DLL 到工程 ====================

        /// <summary>
        /// 把生成的热更 DLL 和 AOT 元数据 DLL 拷贝进工程（.dll.bytes）。
        /// 为什么用 .dll.bytes：Assets 下的原始 .dll 会被 Unity 当预编译插件程序集引用进编译，
        /// 和源码编译的程序集类型重复导致报错；.bytes 按 TextAsset 导入，不会被编译，
        /// 也能被 YooAsset 的 LoadAssetAsync&lt;TextAsset&gt; 读取（AddressByFileName 得到地址仍是 Xxx.dll）。
        /// 拷贝后记得重新构建 YooAsset 资源包并上传。
        /// </summary>
        [MenuItem("FrameWorkTool/HybridCLRTool/7. 拷贝热更DLL + AOT元数据到工程", priority = 70)]
        public static void CopyHotUpdateAndAOTDlls()
        {
            string targetName = EditorUserBuildSettings.activeBuildTarget.ToString();
            var missing = new List<string>();
            int copiedHot = 0;
            int copiedAOT = 0;

            // 1) 热更 DLL：HybridCLRData/HotUpdateDlls/{平台} → Assets/PackageAssets/JITDLL
            foreach (string assemblyName in HotUpdateAssemblyNames)
            {
                copiedHot += CopyToBytes(
                    HotUpdateDllSrcRoot, targetName, assemblyName, HotUpdateDllDestDir, missing);
            }

            // 2) AOT 元数据 DLL：HybridCLRData/AssembliesPostIl2CppStrip/{平台} → Assets/PackageAssets/AOTDLL
            foreach (string assemblyName in AOTAssemblyNames)
            {
                copiedAOT += CopyToBytes(
                    AOTDllSrcRoot, targetName, assemblyName, AOTDllDestDir, missing);
            }

            // 让 Unity 重新导入新拷贝的 .bytes 资源
            AssetDatabase.Refresh();

            if (missing.Count > 0)
            {
                Debug.LogWarning(
                    $"[HybridCLRTool] 拷贝完成：热更 {copiedHot} 个、AOT {copiedAOT} 个；" +
                    $"以下源文件不存在（可能还没生成对应步骤）：\n{string.Join("\n", missing)}");
            }
            else
            {
                Debug.Log(
                    $"[HybridCLRTool] 拷贝完成：热更 {copiedHot} 个、AOT {copiedAOT} 个\n" +
                    $"→ {HotUpdateDllDestDir} / {AOTDllDestDir}");
            }
        }

        /// <summary>
        /// 把 {srcRoot}/{targetName}/{assemblyName}.dll 拷贝为 {destDir}/{assemblyName}.dll.bytes。
        /// </summary>
        /// <returns>是否成功拷贝（源文件不存在返回 0）</returns>
        private static int CopyToBytes(string srcRoot, string targetName, string assemblyName,
            string destDir, List<string> missing)
        {
            string projectRoot = ProjectRoot;
            string srcFile = Path.Combine(projectRoot, srcRoot, targetName, assemblyName + ".dll");
            if (!File.Exists(srcFile))
            {
                missing.Add(srcFile);
                return 0;
            }

            string destFile = Path.Combine(projectRoot, destDir, assemblyName + ".dll.bytes");
            Directory.CreateDirectory(Path.GetDirectoryName(destFile));
            File.Copy(srcFile, destFile, true);
            return 1;
        }
    }
}
