using System;
using    NFramework.ModuleSystem;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// View资源加载组件,这个组件只可能挂载在container之上的级别中。
    /// </summary>
    public class ViewResLoadComponent : ViewComponent, IResLoader
    {
        public ResLoadRecords ResLoadRecords { get; private set; }

        public override void OnViewComponentAwake()
        {
            this.ResLoadRecords = new ResLoadRecords();
            this.ResLoadRecords.Awake();
        }

        public override void OnViewComponentDestroy()
        {
            this.ResLoadRecords.Destroy();
            this.ResLoadRecords = null;
        }

        private string MappingAssetID(string inAssetID)
        {
            return this.GetSystem<UISystem>().MappingAssetID(inAssetID);
        }

        public T Load<T>(string inAssetID) where T : UnityEngine.Object
        {
            var assetID = this.MappingAssetID(inAssetID);
            return ResLoadRecords.Load<T>(assetID);
        }

        public UniTask<T> LoadAsync<T>(string inAssetID) where T : UnityEngine.Object
        {
            var assetID = this.MappingAssetID(inAssetID);
            return ResLoadRecords.LoadAsync<T>(assetID);
        }

        public void LoadAsync<T>(string inAssetID, Action<T> callback) where T : Object
        {
        }

        public void Free<T>(T inObj) where T : UnityEngine.Object
        {
            ResLoadRecords.Free(inObj);
        }
        public void Free(string inAssetID)
        {
            ResLoadRecords.Free(inAssetID);
        }
    }


    public static class ViewResLoadComponentExtensions
    {
        public static T LoadRes<T>(this View inView, string inAssetID) where T : UnityEngine.Object
        {
            if (ViewUtils.GetContainer<Container>(inView, out var container))
            {
                var loaderComponent = ViewUtils.CheckAndAdd<ViewResLoadComponent>(container);
                return loaderComponent.Load<T>(inAssetID);
            }

            throw new System.Exception("view dont have container");
        }

        public static UniTask<T> LoadResAsync<T>(this View inView, string inAssetID) where T : UnityEngine.Object
        {
            if (ViewUtils.GetContainer<Container>(inView, out var container))
            {
                var component = ViewUtils.CheckAndAdd<ViewResLoadComponent>(container);
                return component.LoadAsync<T>(inAssetID);
            }

            throw new System.Exception("view dont have container");
        }

        public static void FreeRes<T>(this View inView, T inObj) where T : UnityEngine.Object
        {
            if (ViewUtils.GetContainer<Container>(inView, out var container))
            {
                var component = ViewUtils.CheckAndAdd<ViewResLoadComponent>(container);
                component.Free(inObj);
                return;
            }

            throw new System.Exception("view dont have container");
        }
    }
}
