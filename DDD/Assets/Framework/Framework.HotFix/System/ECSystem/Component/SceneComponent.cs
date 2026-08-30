using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// SceneComponent，挂载在 Entity 上的场景加载组件，
    /// 统一委托给全局场景管理器 <see cref="ResSystem.SceneLoader"/>（SceneGroup）处理场景的加载、卸载与切换。
    /// </summary>
    public class SceneComponent : Entity, IAwakeSystem, IDestroySystem
    {
        private SceneGroup SceneManager => this.GetSystem<ResSystem>().SceneLoader;

        public void Awake()
        {
            // 场景管理器由 ResSystem 全局持有，这里无需额外初始化
        }

        public void Destroy()
        {
            // 全局场景管理器由 ResSystem 负责释放
        }

        /// <summary>异步加载场景（Single 会切换主场景，Additive 叠加加载）</summary>
        public UniTask<bool> LoadSceneAsync(string scenePath, LoadSceneMode loadMode,
            uint priority = 0, Action<float> onProgress = null)
        {
            return this.SceneManager.LoadSceneAsync(scenePath, loadMode, priority, onProgress);
        }

        /// <summary>异步卸载叠加场景（主场景不能直接卸载）</summary>
        public UniTask<bool> UnloadSceneAsync(string scenePath)
        {
            return this.SceneManager.UnloadSceneAsync(scenePath);
        }

        /// <summary>卸载所有叠加场景（保留主场景）</summary>
        public UniTask UnloadAllAdditiveScenes()
        {
            return this.SceneManager.UnloadAllAdditiveScenes();
        }

        /// <summary>场景是否已加载</summary>
        public bool IsSceneLoaded(string scenePath)
        {
            return this.SceneManager.IsSceneLoaded(scenePath);
        }

        /// <summary>释放所有场景资源（一般只在游戏关闭时调用）</summary>
        public void ReleaseAll()
        {
            this.SceneManager.ReleaseAll();
        }
    }
}