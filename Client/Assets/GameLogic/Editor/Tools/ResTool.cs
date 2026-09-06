using System.IO;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

namespace Game.Editor.Tools
{
    /// <summary>
    /// 资源（YooAsset）工具：编辑器播放模式切换、打开收集器/构建器窗口。
    /// </summary>
    public static class ResTool
    {
        /// <summary>编辑器播放模式存储键（与 YooAssetService.GetPlayMode 保持一致）</summary>
        private const string EditorPlayModePrefKey = "YooAsset.EditorPlayMode";

        // ==================== 编辑器播放模式 ====================

        /// <summary>模拟模式：直接读 Assets 目录，不加载真 bundle，最快</summary>
        [MenuItem("FrameWorkTool/ResTool/编辑器播放模式/模拟模式(读Assets)", priority = 100)]
        public static void SetEditorSimulateMode()
        {
            EditorPrefs.SetInt(EditorPlayModePrefKey, (int)EPlayMode.EditorSimulateMode);
            Debug.Log("[ResTool] 编辑器播放模式：模拟模式（读Assets）");
        }

        [MenuItem("FrameWorkTool/ResTool/编辑器播放模式/模拟模式(读Assets)", true)]
        private static bool ValidateEditorSimulateMode()
        {
            Menu.SetChecked("FrameWorkTool/ResTool/编辑器播放模式/模拟模式(读Assets)",
                EditorPrefs.GetInt(EditorPlayModePrefKey, (int)EPlayMode.EditorSimulateMode) == (int)EPlayMode.EditorSimulateMode);
            return true;
        }

        /// <summary>离线模式：读 StreamingAssets 里的真 bundle（需先构建并拷贝到 StreamingAssets）</summary>
        [MenuItem("FrameWorkTool/ResTool/编辑器播放模式/离线模式(读真bundle)", priority = 101)]
        public static void SetEditorOfflineMode()
        {
            EditorPrefs.SetInt(EditorPlayModePrefKey, (int)EPlayMode.OfflinePlayMode);
            Debug.Log("[ResTool] 编辑器播放模式：离线模式（读StreamingAssets真bundle）");
        }

        [MenuItem("FrameWorkTool/ResTool/编辑器播放模式/离线模式(读真bundle)", true)]
        private static bool ValidateEditorOfflineMode()
        {
            Menu.SetChecked("FrameWorkTool/ResTool/编辑器播放模式/离线模式(读真bundle)",
                EditorPrefs.GetInt(EditorPlayModePrefKey, (int)EPlayMode.EditorSimulateMode) == (int)EPlayMode.OfflinePlayMode);
            return true;
        }

        /// <summary>联机模式：从更新服务器下载真 bundle（需要启动更新服务器）</summary>
        [MenuItem("FrameWorkTool/ResTool/编辑器播放模式/联机模式(下载)", priority = 102)]
        public static void SetEditorHostMode()
        {
            EditorPrefs.SetInt(EditorPlayModePrefKey, (int)EPlayMode.HostPlayMode);
            Debug.Log("[ResTool] 编辑器播放模式：联机模式（下载）");
        }

        [MenuItem("FrameWorkTool/ResTool/编辑器播放模式/联机模式(下载)", true)]
        private static bool ValidateEditorHostMode()
        {
            Menu.SetChecked("FrameWorkTool/ResTool/编辑器播放模式/联机模式(下载)",
                EditorPrefs.GetInt(EditorPlayModePrefKey, (int)EPlayMode.EditorSimulateMode) == (int)EPlayMode.HostPlayMode);
            return true;
        }

        // ==================== YooAsset 窗口 ====================

        /// <summary>打开资源收集器配置窗口（检查/修改收集目录、tag、地址规则）</summary>
        [MenuItem("FrameWorkTool/ResTool/打开 AssetBundle Collector", priority = 200)]
        public static void OpenAssetBundleCollector()
        {
            AssetBundleCollectorWindow.OpenWindow();
        }

        /// <summary>打开资源构建器窗口（构建真 bundle 并拷贝到 StreamingAssets）</summary>
        [MenuItem("FrameWorkTool/ResTool/打开 AssetBundle Builder", priority = 201)]
        public static void OpenAssetBundleBuilder()
        {
            AssetBundleBuilderWindow.OpenWindow();
        }

        /// <summary>
        /// 把最新一次构建的 bundle 拷贝到 StreamingAssets（内置包目录）。
        /// 为什么：离线模式（OfflinePlayMode）从 StreamingAssets 读取真 bundle；
        /// 构建器本身有“拷贝到StreamingAssets”选项，但这里提供一键手动拷贝，
        /// 方便你构建后（或切换版本后）随时同步到内置目录。
        /// 源：Bundles/{平台}/{包名}/{最新版本}（AssetBundleBuilder 的默认输出）
        /// 目标：{StreamingAssets内置根}/{包名}（例如 Assets/StreamingAssets/yoo/DefaultPackage）
        /// 注意：BuildinCatalog.bytes 是构建时由 YooAsset 生成到内置目录的，
        /// 不在构建产物的版本目录里，所以本方法只覆盖拷贝清单和 bundle，不清空目录。
        /// </summary>
        [MenuItem("FrameWorkTool/ResTool/拷贝构建产物到StreamingAssets", priority = 210)]
        public static void CopyBuildToStreamingAssets()
        {
            string buildTarget = EditorUserBuildSettings.activeBuildTarget.ToString();
            const string packageName = "DefaultPackage"; // 与 BootConfig.packageName 一致

            // 构建产物根目录：Bundles/{平台}/{包名}/{版本}
            string packageRoot = Path.Combine(
                AssetBundleBuilderHelper.GetDefaultBuildOutputRoot(), buildTarget, packageName);
            if (!Directory.Exists(packageRoot))
            {
                Debug.LogError($"[ResTool] 找不到构建产物：{packageRoot}\n请先通过 AssetBundle Builder 构建真 bundle。");
                return;
            }

            // 取最新构建的版本目录（按最后修改时间倒序）
            var versionDirs = Directory.GetDirectories(packageRoot);
            if (versionDirs.Length == 0)
            {
                Debug.LogError($"[ResTool] {packageRoot} 下没有版本目录，请先构建。");
                return;
            }

            System.Array.Sort(versionDirs, (a, b) => Directory.GetLastWriteTime(b).CompareTo(Directory.GetLastWriteTime(a)));
            string latestDir = versionDirs[0];

            // 目标：YooAsset 的 StreamingAssets 内置根目录/{包名}
            // 例如 Assets/StreamingAssets/yoo/DefaultPackage
            string buildinRoot = Path.Combine(AssetBundleBuilderHelper.GetStreamingAssetsRoot(), packageName);

            // 只覆盖拷贝清单和 bundle，不清空目录（避免删掉构建时生成的 BuildinCatalog.bytes）
            CopyFolderContents(latestDir, buildinRoot);

            AssetDatabase.Refresh();
            Debug.Log($"[ResTool] 已拷贝构建产物：\n{latestDir}\n→\n{buildinRoot}");

            // BuildinCatalog.bytes 是构建器在“拷贝到StreamingAssets”选项开启时才生成的，
            // 本按钮无法生成它；如果缺失，运行时（离线模式）初始化会 404 报错。
            if (!File.Exists(Path.Combine(buildinRoot, "BuildinCatalog.bytes")))
            {
                Debug.LogWarning(
                    $"[ResTool] 注意：{buildinRoot} 下没有 BuildinCatalog.bytes。\n" +
                    "请用 AssetBundle Builder 构建时勾选“拷贝到StreamingAssets（ClearAndCopyAll）”，" +
                    "构建器会生成 BuildinCatalog.bytes + 清单文件；本按钮只能同步 bundle。");
            }
        }

        /// <summary>递归拷贝目录内容到目标目录</summary>
        private static void CopyFolderContents(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), true);
            }

            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                CopyFolderContents(dir, Path.Combine(destDir, Path.GetFileName(dir)));
            }
        }
    }
}
