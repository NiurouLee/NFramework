using System;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// ResComponent，挂载在 Entity 上的资源加载组件，
    /// 负责管理该实体及其子实体的资源加载与引用计数。
    /// 通过 <see cref="ResLoadRecords"/> 统一记录已加载资源。
    /// </summary>
    public class ResComponent : Entity, IResLoader, IAwakeSystem, IDestroySystem
    {
        public ResLoadRecords ResLoadRecords { get; set; }

        public void Awake()
        {
            if (this.ResLoadRecords == null)
            {
                this.ResLoadRecords = new ResLoadRecords();
                this.ResLoadRecords.Awake();
            }
        }

        public void Destroy()
        {
            if (this.ResLoadRecords != null)
            {
                this.ResLoadRecords.Destroy();
                this.ResLoadRecords = null;
            }
        }

        public object LoadRawAsset(string inAssetID)
        {
            return this.ResLoadRecords.LoadRawAsset(inAssetID);
        }

        public T Load<T>(string inAssetID) where T : Object
        {
            return this.ResLoadRecords.Load<T>(inAssetID);
        }

        public UniTask<T> LoadAsync<T>(string inAssetID) where T : Object
        {
            return this.ResLoadRecords.LoadAsync<T>(inAssetID);
        }

        public void LoadAsync<T>(string inAssetID, Action<T> callback) where T : Object
        {
            this.ResLoadRecords.LoadAsync<T>(inAssetID)
                .ContinueWith(asset => callback?.Invoke(asset))
                .Forget();
        }

        public void Free<T>(T inObj) where T : Object
        {
            this.ResLoadRecords.Free(inObj);
        }

        public void Free(string inAssetID)
        {
            this.ResLoadRecords.Free(inAssetID);
        }
    }
}