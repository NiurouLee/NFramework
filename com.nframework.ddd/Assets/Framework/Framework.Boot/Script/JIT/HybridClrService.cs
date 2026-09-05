using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HybridCLR;
using YooAsset;
using Cysharp.Threading.Tasks;

namespace NFramework.Boot
{
    /*
     * HybridCLR/CompileDll/ActiveBuildTarget —— 编译热更 dll（HotUpdateDlls/{平台}）
     *HybridCLR/Generate/Il2CppDef —— 生成 il2cpp 补丁定义
     *HybridCLR/Generate/LinkXml —— 依赖第 1 步的热更 dll
     *HybridCLR/Generate/AOTDlls —— 剥离 AOT dll（依赖 IL2CPP，内部跑一次构建）
     *HybridCLR/Generate/MethodBridge —— 依赖第 4 步的 AOT dll
     *HybridCLR/Generate/AOTGenericReference —— 依赖第 1 步 + 第 4 步
     */

    public class HybridClrService : IJITServices
    {
        private readonly string _hotUpdateAssemblyName;
        private readonly string _mainClassName;
        private readonly string _mainMethodName;

        // 事件
        public event Action<string> OnStepChange;
        public event Action<string> OnError;

        private Assembly _hotUpdateAssembly;
        private List<AssetHandle> _aotHandles = new List<AssetHandle>();
        private List<AssetHandle> _jitHandles = new List<AssetHandle>();

        public HybridClrService()
        {
            _hotUpdateAssemblyName = BootConfig.hotUpdateDllName;
            _mainClassName = BootConfig.hotUpdateEntryScript;
            _mainMethodName = BootConfig.hotUpdateEntryMethod;
        }

        /// <summary>
        /// 开始HybridCLR更新流程
        /// </summary>
        public async UniTask<bool> StartJITUpdate()
        {
            try
            {
                AOTLog.Info("开始HybridCLR更新流程");
                OnStepChange?.Invoke("开始HybridCLR更新...");

                // 加载AOT元数据
                if (!await LoadAOT())
                    return false;

                // 加载热更dll
                if (!await LoadJIT())
                    return false;

                OnStepChange?.Invoke("HybridCLR更新完成");
                AOTLog.Info("HybridCLR更新流程完成");
                return true;
            }
            catch (Exception e)
            {
                AOTLog.Error($"HybridCLR更新失败: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 加载AOT元数据
        /// </summary>
        public async UniTask<bool> LoadAOT()
        {
#if UNITY_EDITOR
            // 编辑器模式下跳过AOT元数据加载
            OnStepChange?.Invoke("编辑器模式跳过AOT元数据加载");
            AOTLog.Info("编辑器模式跳过AOT元数据加载");
            await UniTask.Yield();
            return true;
#else
            OnStepChange?.Invoke("加载AOT元数据...");
            var locations = YooAssets.GetAssetInfos("AOTDLL");
            AOTLog.Info($"开始加载 {locations.Length} 个AOT DLL文件");

            foreach (var location in locations)
            {
                try
                {
                    var handle = YooAssets.LoadAssetAsync<TextAsset>(location.Address);
                    _aotHandles.Add(handle);
                    await handle.ToUniTask();

                    if (handle.Status == EOperationStatus.Succeed)
                    {
                        TextAsset dllFile = handle.AssetObject as TextAsset;
                        HomologousImageMode mode = HomologousImageMode.SuperSet;
                        LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllFile.bytes, mode);

                        if (err == LoadImageErrorCode.OK)
                        {
                            AOTLog.Info($"成功加载并注册AOT元数据: {location.Address}, mode: {mode}");
                        }
                        else
                        {
                            AOTLog.Error($"注册AOT元数据失败: {location.Address}, 错误码: {err}");
                            return false;
                        }
                    }
                    else
                    {
                        AOTLog.Error($"加载AOT DLL失败: {location.Address}");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    AOTLog.Error($"加载AOT元数据异常: {location.Address}, {e.Message}");
                    return false;
                }
            }

            AOTLog.Info($"AOT元数据加载完成，共 {_aotHandles.Count} 个文件");
            return true;
#endif
        }

        /// <summary>
        /// 加载热更dll
        /// </summary>
        public async UniTask<bool> LoadJIT()
        {
#if UNITY_EDITOR
            OnStepChange?.Invoke("编辑器模式加载HotUpdateDLL...");

            // 编辑器模式：直接从当前域中获取程序集
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => $"{a.GetName().Name}.dll" == _hotUpdateAssemblyName);

            if (assembly != null)
            {
                _hotUpdateAssembly = assembly;
                AOTLog.Info($"编辑器模式HotUpdateDLL加载成功: {_hotUpdateAssemblyName}");
            }
            else
            {
                AOTLog.Error($"编辑器模式找不到HotUpdateDLL: {_hotUpdateAssemblyName}");
                return false;
            }

            await UniTask.Yield();
            return true;
#else
            OnStepChange?.Invoke("加载JIT DLL...");

            // 非编辑器模式：使用YooAsset加载资源
            var locations = YooAssets.GetAssetInfos("JITDLL");
            AOTLog.Info($"开始加载 {locations.Length} 个JITDLL文件");

            foreach (var location in locations)
            {
                try
                {
                    var handle = YooAssets.LoadAssetAsync<TextAsset>(location.Address);
                    _jitHandles.Add(handle);
                    await handle.ToUniTask();

                    if (handle.Status == EOperationStatus.Succeed)
                    {
                        TextAsset dllFile = handle.AssetObject as TextAsset;
                        Assembly loadedAssembly = Assembly.Load(dllFile.bytes);

                        if (location.Address.Contains(_hotUpdateAssemblyName))
                        {
                            _hotUpdateAssembly = loadedAssembly;
                            AOTLog.Info($"设置主热更程序集: {location.Address}");
                        }

                        AOTLog.Info($"成功加载JIT DLL: {location.Address}");
                    }
                    else
                    {
                        AOTLog.Error($"加载JIT DLL资源失败: {location.Address}");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    AOTLog.Error($"加载程序集失败: {location.Address}, 错误: {e.Message}");
                    return false;
                }
            }

            AOTLog.Info($"JIT DLL加载完成，共 {_jitHandles.Count} 个文件");
            return true;
#endif
        }

        /// <summary>
        /// 进入主入口
        /// </summary>
        public async UniTask<bool> EnterMainEntry()
        {
            OnStepChange?.Invoke("进入主游戏入口...");

            if (_hotUpdateAssembly == null)
            {
                AOTLog.Error("热更程序集未加载，无法进入主入口");
                return false;
            }

            try
            {
                Type type = _hotUpdateAssembly.GetType(_mainClassName);
                if (type == null)
                {
                    AOTLog.Error($"找不到主脚本类型: {_mainClassName}");
                    return false;
                }

                MethodInfo method = type.GetMethod(_mainMethodName, BindingFlags.Public | BindingFlags.Static);
                if (method == null)
                {
                    AOTLog.Error($"找不到主方法: {_mainMethodName} 在类型 {_mainClassName} 中");
                    return false;
                }

                AOTLog.Info($"调用热更入口: {_mainClassName}.{_mainMethodName}");

                if (method.ReturnType == typeof(UniTask))
                {
                    // 异步方法
                    UniTask invokeTask = (UniTask)method.Invoke(null, null);
                    await invokeTask;
                }
                else
                {
                    // 同步方法
                    method.Invoke(null, null);
                }

                return true;
            }
            catch (Exception e)
            {
                AOTLog.Error($"执行热更入口失败: {e}");
                return false;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            AOTLog.Info("开始释放HybridCLR资源");
            // 释放所有资源句柄
            foreach (var handle in _aotHandles)
            {
                if (handle.IsValid)
                    handle.Release();
            }

            foreach (var handle in _jitHandles)
            {
                if (handle.IsValid)
                    handle.Release();
            }

            _aotHandles.Clear();
            _jitHandles.Clear();

            AOTLog.Info("HybridCLRManager 资源已释放");
        }
    }
}