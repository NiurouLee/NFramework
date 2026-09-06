using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using YooAsset;
using   NFramework;
using  NFramework.Core;

namespace NFramework.ModuleSystem
{

    /// <summary>
    /// 资源组抽象基类
    /// 提供资源加载的基础功能，子类实现不同的释放策略
    /// </summary>
    public class ResourceGroupBase : NObject
    {
        #region 字段

        /// <summary>
        /// Asset到Handle的映射
        /// </summary>
        protected Dictionary<UnityEngine.Object, AssetHandle> _assetToHandle;

        /// <summary>
        /// GameObject实例到Handle的映射
        /// </summary>
        protected Dictionary<GameObject, AssetHandle> _gameObjectToHandle;
        #endregion


        public ResourceGroupBase()
        {

        }


        #region 构造函数
        public void Awake()
        {
            this._assetToHandle = DictionaryPool.Alloc<UnityEngine.Object, AssetHandle>();
            this._gameObjectToHandle = DictionaryPool.Alloc<GameObject, AssetHandle>();
        }

        #endregion

        #region 异步加载

        /// <summary>
        /// 异步加载资源
        /// </summary>
        public async UniTask<T> LoadAssetAsync<T>(string assetPath, uint priority) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                this.Log?.Print("[ResourceGroup] 资源路径为空");
                return null;
            }

            // 调用 YooAsset 加载
            AssetHandle handle = YooAssets.LoadAssetAsync<T>(assetPath, priority);
            await handle.ToUniTask();

            if (handle.Status != EOperationStatus.Succeed)
            {
                this.Log?.Print("[ResourceGroup] 加载资源失败: {0}, 错误: {1}", assetPath, handle.LastError);
                return null;
            }

            T asset = handle.AssetObject as T;
            if (asset != null)
            {
                _assetToHandle[asset] = handle;
            }

            return asset;
        }

        /// <summary>
        /// 异步加载并实例化GameObject
        /// </summary>
        public async UniTask<GameObject> LoadGameObjectAsync(string assetPath, uint priority = 0, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                this.Error?.Print("[ResourceGroup] 资源路径为空");
                return null;
            }

            // 调用 YooAsset 加载
            AssetHandle handle = YooAssets.LoadAssetAsync<GameObject>(assetPath, priority);
            await handle.ToUniTask();

            if (handle.Status != EOperationStatus.Succeed)
            {
                this.Error?.Print("[ResourceGroup] 加载GameObject失败: {0}, 错误: {1}", assetPath, handle.LastError);
                return null;
            }

            // 实例化
            GameObject prefab = handle.AssetObject as GameObject;
            if (prefab == null)
            {
                this.Error?.Print("[ResourceGroup] 资源类型不是GameObject: {0}", assetPath);
                handle.Release();
                return null;
            }

            GameObject instance = handle.InstantiateSync(position, rotation, parent);
            _gameObjectToHandle[instance] = handle;

            return instance;
        }


        public T LoadAssetSync<T>(string assetPath) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                this.Error?.Print("[ResourceGroup] 资源路径为空");
                return null;
            }

            // 调用 YooAsset 加载
            AssetHandle handle = YooAssets.LoadAssetSync<T>(assetPath);
            if (handle.Status != EOperationStatus.Succeed)
            {
                this.Error?.Print("[ResourceGroup] 加载资源失败: {0}, 错误: {1}", assetPath, handle.LastError);
                return null;
            }

            T asset = handle.AssetObject as T;
            if (asset != null)
            {
                _assetToHandle[asset] = handle;
            }

            return asset;
        }


        #endregion

        #region 立即释放所有资源

        /// <summary>
        /// 立即释放所有资源（无延迟）
        /// </summary>
        public virtual void ReleaseAll()
        {
            // 释放所有Asset
            foreach (var kvp in _assetToHandle)
            {
                kvp.Value?.Release();
            }
            _assetToHandle.Clear();

            // 销毁并释放所有GameObject
            foreach (var kvp in _gameObjectToHandle)
            {
                if (kvp.Key != null)
                {
                    UnityEngine.Object.Destroy(kvp.Key);
                }
                kvp.Value?.Release();
            }
            _gameObjectToHandle.Clear();

            this.Log?.Print("[ResourceGroup] 已释放所有资源");
        }

        #endregion

        public virtual bool HasAsset(string assetPath)
        {
            return YooAssets.CheckLocationValid(assetPath);
        }


    }

}
