
using System;

namespace NFramework.ModuleSystem
{
    public enum ResHandlerState
    {
        None,
        Loading,
        Loaded,
        Failed,
        Cancel,
        Destroy,
    }

    public class ResHandler<T> : ResHandler where T : UnityEngine.Object
    {
        public new T AssetObject => base.AssetObject as T;
    }

    public class ResHandler : NObject
    {
        public string assetID;
        
        public event Action<ResHandler> OnComplete;
        public System.Object AssetObject { get; private set; }
        public ResHandlerState state { get; private set; }

        public void Awake(string inAssetID)
        {
            assetID = inAssetID;
            state = ResHandlerState.Loading;
        }

        public void SetResult(System.Object inObj)
        {
            state = ResHandlerState.Loaded;
            this.AssetObject = inObj;
            OnComplete?.Invoke(this);
        }
        public void SetFailed(System.Object inObj)
        {
            state = ResHandlerState.Failed;
            this.AssetObject = inObj;
            OnComplete?.Invoke(this);
        }

        public void Cancel()
        {
            this.state = ResHandlerState.Cancel;

        }

        public void Destroy()
        {
        }
    }
}