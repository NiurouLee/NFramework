using System;
using NFramework;
using NFramework.Boot;
using NFramework.ModuleSystem;
using UnityEngine;
using YooAsset;
using NFramework.Game.ConfigReader;


namespace Game.Logic
{
    /// <summary>
    /// 作为热更的主入口,和游戏热更逻辑的主要访问点
    /// </summary>
    public partial class Game
    {
        /// <summary>
        /// 热更层入口（AOT 层调用）
        /// </summary>
        public static void Start()
        {
            var go = new GameObject("NFROOT");
            go.gameObject.AddComponent<EngineLoop>();
            GameObject.DontDestroyOnLoad(go);
            NFROOT.AwakeRoot();
            AwakeSystemConfig();
            AwakeSystem();
            StartGameLogic();
        }

        public static void AwakeSystemConfig()
        {
            var handle = YooAssets.LoadAssetSync<FrameworkConfig>(BootConfig.FrameworkConfigAssetName);
            if (handle.Status != EOperationStatus.Succeed || handle.AssetObject == null)
            {
                Debug.LogError(
                    $"[Game] FrameworkConfig 加载失败: {BootConfig.FrameworkConfigAssetName}, Status:{handle.Status}");
                return;
            }

            NFROOT.Instance.Config = handle.AssetObject as FrameworkConfig;
        }

        public static void AwakeSystem()
        {
            RegisterConfig();
            var ViewConfigReader = new ViewConfigReader();
            ViewConfigReader.Initialize();
            NFROOT.I.GetSystem<UISystem>()
                .RegisterConfig(ViewConfigReader.ConfigMap, ViewTypeRegistryAuto.TypeToConfigIDMap);
        }


        public static void RegisterConfig()
        {
            Func<System.Type, IConfig> readerFactory = type =>
            {
                if (TablesReader.Registry.TryGetValue(type, out var info))
                {
                    return info.Create();
                }

                return null;
            };
            Func<System.Type, string> assetIDFactory = type =>
            {
                if (TablesReader.Registry.TryGetValue(type, out var info))
                {
                    return info.AssetName;
                }

                return string.Empty;
            };
            NFROOT.I.GetSystem<ConfigSystem>().readerRegister = readerFactory;
            NFROOT.I.GetSystem<ConfigSystem>().assetInfoRegister = assetIDFactory;
        }

        public static T GetSystem<T>() where T : FrameworkSystemModuleBase, new()
        {
            return NFROOT.I.GetSystem<T>();
        }

        public static T GetLogicModule<T>() where T : class, IGameLogicModule
        {
            return NFROOT.I.GetSystem<GameLogicSystem>().GetLogicModule<T>();
        }

        public static T GetWorld<T>() where T : World, new()
        {
            return NFROOT.I.GetSystem<WorldSystem>().GetWorld<T>();
        }

        public static T GetContext<T>() where T : Context, new()
        {
            return NFROOT.I.GetSystem<ContextSystem>().GetContext<T>();
        }

        public UISystem UISystem => NFROOT.I.GetSystem<UISystem>();
    }
}
