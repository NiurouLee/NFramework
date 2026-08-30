using System;
using Cysharp.Threading.Tasks;
using NFramework;
using NFramework.ModuleSystem;
using UnityEngine.SceneManagement;

namespace Game.Logic
{
    /// <summary>
    /// 示例 World：负责进入游戏后的场景加载与切换。
    /// </summary>
    public class ExWorld : World, IAwakeSystem, IDestroySystem
    {
        /// <summary>
        /// GameScene 场景地址（YooAsset DefaultPackage 收集规则 AddressByFileName）
        /// </summary>
        public const string GameScenePath = "GameScene";

        public SceneComponent SceneComponent { get; private set; }

        public void Awake()
        {
            this.SceneComponent = this.AddComponent<SceneComponent>();
        }

        public UniTask LoadScene()
        {
            this.SceneComponent.LoadSceneAsync(GameScenePath, LoadSceneMode.Single);
            return UniTask.CompletedTask;
        }

        public void Destroy()
        {
        }
    }
}