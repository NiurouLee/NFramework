using System;
using System.Collections.Generic;

namespace NFramework.ModuleSystem
{
    public class ResSystem : FrameworkSystemModuleBase
    {
        private NormalResourceGroup _resourceLorder;

        private SceneGroup _sceneLoader;

        /// <summary>全局场景管理器</summary>
        public SceneGroup SceneLoader => _sceneLoader;

        public override void Awake()
        {
            this._resourceLorder = new NormalResourceGroup();
            this._resourceLorder.Awake();
            this._sceneLoader = new SceneGroup();
            this._sceneLoader.Awake();
        }

        public Dictionary<string, string> AssetID2PathDic = new Dictionary<string, string>();

        public string Address(string inAssetId)
        {
            return inAssetId;
        }

        public ResHandler Createhandler(string inAssetPath)
        {
            var handler = new ResHandler();
            handler.Awake(inAssetPath);
            return handler;
        }

        public ResHandler<T> Createhandler<T>(string inAssetPath) where T : UnityEngine.Object
        {
            var handler = new ResHandler<T>();
            handler.Awake(inAssetPath);
            return handler;
        }

        public void AwakeAssetID2PathMap(List<Tuple<string, string>> inCfgList)
        {
            this.AssetID2PathDic = new Dictionary<string, string>();
            foreach (var item in inCfgList)
            {
                this.AssetID2PathDic.Add(item.Item1, item.Item2);
            }
        }

        public ResHandler LoadRawAsset(string inAssetID)
        {
            var asset = YooAsset.YooAssets.LoadRawFileSync(inAssetID).GetRawFileData();
            var handler = this.Createhandler(inAssetID);
            handler.SetResult(asset);
            return handler;
        }

        public ResHandler Load<T>(string inAssetID) where T : UnityEngine.Object
        {
            var address = this.Address(inAssetID);
            var handle = this.Createhandler(address);
            var asset = YooAsset.YooAssets.LoadAssetSync<T>(address);

            {
                this.ResloveHander<T>(handle, asset);
                return handle;
            }
        }

        public ResHandler<T> LoadAsync<T>(string inAssetID, uint priority) where T : UnityEngine.Object
        {
            var resPath = this.Address(inAssetID);
            var handler = this.Createhandler<T>(resPath);
            var yooAssetHandle = YooAsset.YooAssets.LoadAssetAsync<T>(resPath);
            yooAssetHandle.Completed += (handle) =>
            {
                if (handle.IsDone && handle.AssetObject != null)
                {
                    this.ResloveHander<T>(handler, handle);
                }
                else
                {
                    handler.SetFailed(null);
                }
            };
            return handler as ResHandler<T>;
        }


        private void ResloveHander<T>(ResHandler inHandler, T inAsset) where T : UnityEngine.Object
        {
            inHandler.SetResult(inAsset);
        }

        private void ResloveHander<T>(ResHandler inHandler, YooAsset.AssetHandle inYooAssetHandle)
            where T : UnityEngine.Object
        {
            if (inYooAssetHandle.Status != YooAsset.EOperationStatus.Succeed)
            {
                inHandler.SetFailed(null);
                Error?.Print($"加载资源失败: {inHandler.assetID}, 错误: {inYooAssetHandle.LastError}");
                return;
            }

            var asset = inYooAssetHandle.AssetObject as T;
            if (asset == null)
            {
                inHandler.SetFailed(null);
                return;
            }

            inHandler.SetResult(asset);
        }

        public void Free(ResHandler inHandler)
        {
        }

        public void Free(UnityEngine.Object inObj)
        {
        }

        public bool HasResID(string inAssetID)
        {
            return this._resourceLorder.HasAsset(inAssetID);
        }
    }
}
