using System;
using Codice.Client.BaseCommands.BranchExplorer;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// uiFacade 提供者
    /// </summary>
    public class UIFacadeProviderDynamic : ViewComponent, IUIFacadeProvider
    {
        private IResLoader m_ResLoader;

        public override void Awake(View inView,string inName)
        {
            base.Awake(inView,inName);
            m_ResLoader = ViewUtils.CheckAndAdd<ViewResLoadComponent>(inView);
        }

        public UIFacade Alloc<T>() where T : View
        {
            var viewConfig = this.GetSystem<UISystem>().GetViewConfig<T>();
            var viewID = viewConfig.ID;
            return this.Alloc(viewID);
        }

        public UIFacade Alloc(string inViewID)
        {
            var viewConfig = this.GetSystem<UISystem>().GetViewConfig(inViewID);
            var assetId = viewConfig.AssetID;
            var go = this.m_ResLoader.Load<GameObject>(assetId);
            var goIns = UnityEngine.Object.Instantiate(go);
            return goIns.GetComponent<UIFacade>();
        }

        public UniTaskCompletionSource<UIFacade> AllocAsync<T>() where T : View
        {
            var viewConfig = this.GetSystem<UISystem>().GetViewConfig<T>();
            var viewId = viewConfig.ID;
            var deferred = this.AllocAsync(viewId);
            return deferred;
        }

        public UniTaskCompletionSource<UIFacade> AllocAsync(string inViewID)
        {
            var viewConfig = this.GetSystem<UISystem>().GetViewConfig(inViewID);
            var assetId = viewConfig.AssetID;
            var deferred = new UniTaskCompletionSource<UIFacade>();
            this.InstantiateAsync(assetId, deferred);
            return deferred;
        }

        private async void InstantiateAsync(string inAssetID, UniTaskCompletionSource<UIFacade> inDeferred)
        {
            this.View.AddPromise(inDeferred);
            try
            {
                var go = await this.m_ResLoader.LoadAsync<GameObject>(inAssetID);
                if (go == null)
                {
                    inDeferred.TrySetCanceled();
                    return;
                }

                var goIns = UnityEngine.Object.Instantiate(go);
                inDeferred.TrySetResult(goIns.GetComponent<UIFacade>());
            }
            catch (OperationCanceledException)
            {
                inDeferred.TrySetCanceled();
            }
            catch (System.Exception e)
            {
                this.GetSystem<LoggerSystem>()
                    ?.ErrStack($"UIFacadeProviderDynamic load facade failed, AssetID:{inAssetID}, Error:{e}");
                inDeferred.TrySetCanceled();
            }
        }

        public void Destroy()
        {
        }

        public void Free(UIFacade inUIFacade)
        {
        }
    }
}