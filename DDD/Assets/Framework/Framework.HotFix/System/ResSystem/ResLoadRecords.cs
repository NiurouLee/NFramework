using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using  NFramework.Core;
using Object = UnityEngine.Object;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 资源加载记录,这一层会cache count,引用计数
    /// </summary>
    public class ResLoadRecords : BaseRecordMap<string, ResHandler>, IResLoader
    {
        private Dictionary<object, ResHandler> _loadedMap;
        private Dictionary<string, ResHandler> _loadingMap;
        private Dictionary<string, int> _ResRefCount;
        public UnOrderMultiMapLink<string, ICancelPromise> _loadingPromises;


        protected override void OnAwake()
        {
            this._loadedMap = DictionaryPool.Alloc<object, ResHandler>();
            this._loadingMap = DictionaryPool.Alloc<string, ResHandler>();
            this._ResRefCount = DictionaryPool.Alloc<string, int>();
            this._loadingPromises = new UnOrderMultiMapLink<string, ICancelPromise>();
        }

        protected override void OnDestroy()
        {
            this._loadedMap.Clear();
            DictionaryPool.Free(this._loadedMap);
            this._loadingMap.Clear();
            DictionaryPool.Free(this._loadingMap);
            this._ResRefCount.Clear();
            DictionaryPool.Free(this._ResRefCount);
            this._loadingPromises.Clear();
            this._loadingPromises = null;
        }

        public System.Object LoadRawAsset(string inAssetID)
        {
            if (!CheckResID(inAssetID))
            {
                return null;
            }
            var handle = ((NObject)NFROOT.I).GetSystem<ResSystem>().LoadRawAsset(inAssetID);
            if (handle.state == ResHandlerState.Loaded)
            {
                return handle.AssetObject;
            }
            return null;
        }



        public T Load<T>(string inAssetID) where T : Object
        {
            if (!CheckResID(inAssetID))
            {
                return null;
            }
            AddRef(inAssetID);
            if (this.TryGet(inAssetID, out var handler))
            {
                this._ResRefCount[inAssetID]++;
                return handler.AssetObject as T;
            }
            //先异步加载又同步加载的话 不能空转，这样加载资源的逻辑会泡不到，所以不能while(true)
            //yoo底层处理了。
            var handle = ((NObject)NFROOT.I).GetSystem<ResSystem>().Load<T>(inAssetID);
            if (handle.state == ResHandlerState.Loaded)
            {
                this._loadedMap.TryAdd(inAssetID, handle);
                return handle.AssetObject as T;
            }
            else
            {
                //todo:按理说不应该来到这里
                throw new System.Exception("load failed");
            }
            return null;
        }

        public void LoadAsync<T>(string inAssetID, Action<T> callback) where T : Object
        {
            throw new NotImplementedException();
        }

        public UniTask<T> LoadAsync<T>(string inAssetID) where T : Object
        {
            if (!CheckResID(inAssetID))
            {
                return UniTask.FromResult<T>(null);
            }
            AddRef(inAssetID);
            var deferred = new UniTaskCompletionSource<T>();
            if (this.TryGet(inAssetID, out var handler))
            {
                deferred.TrySetResult(handler.AssetObject as T);
                return deferred.Task;
            }
            if (this._loadingMap.TryGetValue(inAssetID, out var loadingHandler))
            {
                this._loadingPromises.Add(inAssetID, deferred);
                return deferred.Task;
            }
            //从未加载过
            else
            {
                var loadHandler = ((NObject)NFROOT.I).GetSystem<ResSystem>().LoadAsync<T>(inAssetID, 0);
                this._loadingMap.Add(inAssetID, loadHandler);
                this._loadingPromises.Add(inAssetID, deferred);
                loadHandler.OnComplete += callback;
                return deferred.Task;
            }

            void callback(ResHandler inHandler)
            {
                this.onHandleComplete<T>(inHandler);
            }
        }

        private void onHandleComplete<T>(ResHandler inHandler) where T : Object
        {
            if (inHandler.state == ResHandlerState.Loaded)
            {
                this.TryAdd(inHandler.assetID, inHandler);
                this._loadedMap.Add(inHandler.AssetObject, inHandler);
                var link = this._loadingPromises[inHandler.assetID];
                if (link.Count > 0)
                {
                    var first = link.First;
                    while (first != null && first.Value != null)
                    {
                        var deferred = (UniTaskCompletionSource<T>)first.Value;
                        deferred.TrySetResult(inHandler.AssetObject as T);
                        first = first.Next;
                    }
                }
                this._loadingPromises.RemoveAll(inHandler.assetID);
                this._loadingMap.Remove(inHandler.assetID);
            }
            else
            {
                this.CancelLoadingPromises(inHandler.assetID);
            }
        }

        private void CancelLoadingPromises(string inAssetID)
        {
            var link = this._loadingPromises[inAssetID];
            var first = link.First;
            while (first != null && first.Value != null)
            {
                first.Value.TrySetCanceled();
                first = first.Next;
            }
            this._loadingPromises.RemoveAll(inAssetID);
            this._loadingMap.Remove(inAssetID);
        }

        private bool CheckResID(string inAssetID)
        {
            if (this._loadedMap.ContainsKey(inAssetID))
            {
                return true;
            }
            if (this._loadingMap.ContainsKey(inAssetID))
            {
                return true;
            }
            if (this._ResRefCount.ContainsKey(inAssetID))
            {
                return true;
            }
            if (string.IsNullOrEmpty(inAssetID))
            {
                return false;
            }
            if (!((NObject)NFROOT.I).GetSystem<ResSystem>().HasResID(inAssetID))
            {
                return false;
            }
            return true;
        }

        private void AddRef(string inAssetID)
        {
            if (this._ResRefCount.TryGetValue(inAssetID, out var refCount) && refCount > 0)
            {
                refCount++;
            }
            else
            {
                this._ResRefCount[inAssetID] = 1;
            }
        }
        private void RemoveRef(string inAssetID)
        {
            if (this._ResRefCount.TryGetValue(inAssetID, out var refCount) && refCount > 0)
            {
                refCount--;
                if (refCount == 0)
                {
                    if (this._loadingMap.TryGetValue(inAssetID, out var handler))
                    {
                        this.CancelLoadingPromises(inAssetID);
                        ((NObject)NFROOT.I).GetSystem<ResSystem>().Free(handler);
                    }
                    this._ResRefCount.Remove(inAssetID);
                    this.TryRemove(inAssetID);
                }
            }
        }


        public void Free(string inAssetID)
        {
            foreach (var handler in this._loadedMap.Values)
            {
                if (handler.assetID == inAssetID)
                {
                    this.RemoveRef(inAssetID);
                }
            }
        }

        public void Free<T>(T inObj) where T : UnityEngine.Object
        {
            if (inObj == null)
            {
                ((NObject)NFROOT.I).GetSystem<LoggerSystem>().Error?.Print("inObj is null");
                return;
            }
            foreach (var handler in this._loadedMap.Values)
            {
                if (handler.AssetObject == inObj)
                {
                    this.RemoveRef(handler.assetID);
                    break;
                }
            }
        }
    }
}
