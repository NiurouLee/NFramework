using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace NFramework.ModuleSystem
{
    public partial class UISystem
    {
        #region Cache

        /// <summary>
        /// 缓存打开中、打开的windowReq
        /// </summary>
        public Dictionary<string, WindowRequest> WindowRequestDictionary = new Dictionary<string, WindowRequest>();

        private bool CheckWindowReq(ViewConfig inViewConfig, out WindowRequest outWindowRequest)
        {
            var windowID = inViewConfig.ID;
            if (this.WindowRequestDictionary.TryGetValue(windowID, out var request))
            {
                outWindowRequest = request;
                return true;
            }
            else
            {
                outWindowRequest = null;
                return false;
            }
        }

        private bool CheckWindowReq(string inWindowID, out WindowRequest outWindowRequest)
        {
            if (this.WindowRequestDictionary.TryGetValue(inWindowID, out var request))
            {
                outWindowRequest = request;
                return false;
            }
            else
            {
                outWindowRequest = null;
                return true;
            }
        }

        #endregion

        private void AddWindowRequest(string inWindowID, WindowRequest inWindowRequest)
        {
            WindowRequestDictionary.Add(inWindowID, inWindowRequest);
        }

        private bool RemoveWindowRequest(string inWindowID)
        {
            return WindowRequestDictionary.Remove(inWindowID);
        }


        #region common

        private WindowRequest CreateRequestAndWindow<TW>(ViewConfig inViewConfig) where TW : Window, new()
        {
            var windowRequest = CreateRequest<TW>(inViewConfig);
            var window = this.CreateView<TW>();
            windowRequest.CacheWindow(window);
            windowRequest.SetStage(WindowRequestStage.Cache);
            return windowRequest;
        }

        private WindowRequestByWindow<TW> CreateRequest<TW>(ViewConfig inViewConfig) where TW : Window
        {
            var windowRequest = new WindowRequestByWindow<TW>(inViewConfig);
            AddWindowRequest(inViewConfig.ID, windowRequest);
            windowRequest.SetStage(WindowRequestStage.Construct);
            return windowRequest;
        }

        private WindowRequest CreateRequestAndWindow<TW, TD>(ViewConfig inViewConfig, TD inViewData)
            where TW : Window, IViewSetData<TD>, new() where TD : class
        {
            var windowRequest = CreateRequest<TW, TD>(inViewConfig, inViewData);
            var window = this.CreateView<TW>();
            windowRequest.CacheWindowAndData(window, inViewData);
            windowRequest.SetStage(WindowRequestStage.Cache);
            return windowRequest;
        }

        private WindowRequest<TW, TD> CreateRequest<TW, TD>(ViewConfig inViewConfig, TD inViewData)
            where TW : Window, IViewSetData<TD>, new() where TD : class
        {
            var windowRequest = new WindowRequest<TW, TD>(inViewConfig);
            AddWindowRequest(inViewConfig.ID, windowRequest);
            windowRequest.SetStage(WindowRequestStage.Construct);
            return windowRequest;
        }


        private WindowRequestByWindow CreateRequest(ViewConfig inViewConfig)
        {
            var windowRequest = new WindowRequestByWindow(inViewConfig);
            AddWindowRequest(inViewConfig.ID, windowRequest);
            windowRequest.SetStage(WindowRequestStage.Construct);
            var window = this.CreateView(inViewConfig) as Window;
            windowRequest.Setup(window);
            windowRequest.SetStage(WindowRequestStage.Cache);
            return windowRequest;
        }


        private WindowRequestByData<TD> CreateRequestByData<TD>(ViewConfig inViewConfig, TD inViewData) where TD : class
        {
            var windowRequest = new WindowRequestByData<TD>(inViewConfig);
            AddWindowRequest(inViewConfig.ID, windowRequest);
            windowRequest.SetStage(WindowRequestStage.Construct);
            var window = this.CreateView(inViewConfig) as Window;
            windowRequest.CacheWindowAndData(window, inViewData);
            windowRequest.SetStage(WindowRequestStage.Cache);
            return windowRequest;
        }

        #endregion

        #region Async

        private UniTask _OpenAsync<TW>(ViewConfig inViewConfig) where TW : Window, new()
        {
            WindowRequest windowRequest = null;
            // 正在打开或者已经打开了
            if (CheckWindowReq(inViewConfig, out var outWindowRequest))
            {
                return outWindowRequest.Deferred.Task;
            }
            else if (this.m_Pool.TryGetWindow<TW>(inViewConfig.ID, out var window))
            {
                windowRequest = CreateRequest(inViewConfig);
                windowRequest.CacheWindow(window);
                windowRequest.SetStage(WindowRequestStage.Cache);
            }
            else
            {
                windowRequest = CreateRequestAndWindow<TW>(inViewConfig);
            }
            var deferred = new UniTaskCompletionSource();
            windowRequest.SetTaskCompletionSource(deferred);
            ___AsyncSetFacade(inViewConfig, windowRequest, deferred);
            return deferred.Task;
        }

        private UniTask _OpenAsync<TD>(ViewConfig inViewConfig, TD inViewData) where TD : class
        {
            WindowRequest windowRequest = null;
            bool isFromPool = false;
            if (CheckWindowReq(inViewConfig, out var outWindowRequest))
            {
                return outWindowRequest.Deferred.Task;
            }
            else if (this.m_Pool.TryGetWindow(inViewConfig.ID, out var window))
            {
                isFromPool = true;
                windowRequest = CreateRequest(inViewConfig);
                windowRequest.CacheWindow(window);
                windowRequest.SetStage(WindowRequestStage.Cache);
            }
            else
            {
                windowRequest = CreateRequestByData<TD>(inViewConfig, inViewData);
            }
            var deferred = new UniTaskCompletionSource();
            windowRequest.SetTaskCompletionSource(deferred);
            ___AsyncSetFacade(inViewConfig, windowRequest, deferred, isFromPool);
            return deferred.Task;
        }

        private UniTask _OpenAsync<TW, TD>(ViewConfig inViewConfig, TD inViewData)
            where TW : Window, IViewSetData<TD>, new() where TD : class
        {
            if (CheckWindowReq(inViewConfig, out var outWindowRequest))
            {
                return outWindowRequest.Deferred.Task;
            }
            else
            {
                var windowRequest = CreateRequestAndWindow<TW, TD>(inViewConfig, inViewData);
                var deferred = new UniTaskCompletionSource();
                windowRequest.SetTaskCompletionSource(deferred);
                ___AsyncSetFacade(inViewConfig, windowRequest, deferred);
                return deferred.Task;
            }
        }

        private UniTask _OpenAsync(ViewConfig inViewConfig)
        {
            if (CheckWindowReq(inViewConfig, out var outWindowRequest))
            {
                return outWindowRequest.Deferred.Task;
            }
            else
            {
                WindowRequestByWindow windowRequest = CreateRequest(inViewConfig);
                var deferred = new UniTaskCompletionSource();
                windowRequest.SetTaskCompletionSource(deferred);
                ___AsyncSetFacade(inViewConfig, windowRequest, deferred);
                return deferred.Task;
            }
        }


        private async void ___AsyncSetFacade(ViewConfig inViewConfig, WindowRequest inWindowRequest, UniTaskCompletionSource inDeferred, bool isFromPool = false)
        {
            UIFacade loadFacade = null;
            if (isFromPool)
            {
                // inWindowRequest.CacheWindow(window);
                // inWindowRequest.CacheProvider(providerDynamic);
                // inWindowRequest.SetStage(WindowRequestStage.FacadeLoading);
            }
            else
            {
                var windowFacadeProvider = this.___CreateWindowFacadeProvider(inWindowRequest.CacheWindowObj);
                inWindowRequest.CacheProvider(windowFacadeProvider);
                inWindowRequest.SetStage(WindowRequestStage.FacadeLoading);
                try
                {
                    loadFacade = await windowFacadeProvider.AllocAsync(inViewConfig.ID).Task;
                }
                catch (OperationCanceledException)
                {
                    inDeferred.TrySetCanceled();
                    return;
                }
                catch (System.Exception e)
                {
                    this.GetSystem<LoggerSystem>()?.ErrStack($"OpenWindow facade load failed, WindowName:{inWindowRequest.Name}, Error:{e}");
                    inDeferred.TrySetCanceled();
                    return;
                }
            }
            ____AsyncSetFacade(inViewConfig, inWindowRequest, loadFacade, inDeferred, isFromPool);
        }

        private void ____AsyncSetFacade(ViewConfig inViewConfig, WindowRequest inWindowRequest, UIFacade loadFacade,
            UniTaskCompletionSource inDeferred, bool isFromPool)
        {
            inWindowRequest.CacheFacade(loadFacade);
            inWindowRequest.SetStage(WindowRequestStage.FacadeLoaded);
            this.__windowSetupCanvas(loadFacade);
            inWindowRequest.SetStage(WindowRequestStage.Layer);
            this.__WindowSetUpLayer(inViewConfig, inWindowRequest.CacheWindowObj, loadFacade);
            this.__windowSetupRectTransform(loadFacade);
            inWindowRequest.SetStage(WindowRequestStage.WindowAwake);
            if (!isFromPool)
            {
                inWindowRequest.Awake();
            }

            inWindowRequest.SetStage(WindowRequestStage.WindowOpen);
            inWindowRequest.Show();
            inDeferred.TrySetResult();
        }

        #endregion

        #region Sync

        private TW _OpenSync<TW>(ViewConfig inViewConfig) where TW : Window, new()
        {
            if (CheckWindowReq(inViewConfig, out var outWindowRequest))
            {
                return null;
            }
            else
            {
                var windowRequest = CreateRequestAndWindow<TW>(inViewConfig);
                return ___SyncSetFacade(inViewConfig, windowRequest) as TW;
            }
        }

        private Window _OpenSync<TD>(ViewConfig inViewConfig, TD inViewData) where TD : class
        {
            if (CheckWindowReq(inViewConfig, out var outWindowRequest))
            {
                return null;
            }
            else
            {
                var windowRequest = CreateRequestByData<TD>(inViewConfig, inViewData);
                return ___SyncSetFacade(inViewConfig, windowRequest);
            }
        }


        private TW _OpenSync<TW, TD>(ViewConfig inViewConfig, TD inViewData)
            where TW : Window, IViewSetData<TD>, new() where TD : class
        {
            if (CheckWindowReq(inViewConfig, out var outWindowRequest))
            {
                return null;
            }
            else
            {
                var windowRequest = CreateRequestAndWindow<TW>(inViewConfig);
                return ___SyncSetFacade(inViewConfig, windowRequest) as TW;
            }
        }

        private Window _OpenSync(ViewConfig inViewConfig)
        {
            if (CheckWindowReq(inViewConfig, out var outWindowRequest))
            {
                return null;
            }
            else
            {
                var windowRequest = CreateRequest(inViewConfig);
                return ___SyncSetFacade(inViewConfig, windowRequest);
            }
        }

        private Window ___SyncSetFacade(ViewConfig inViewConfig, WindowRequest inWindowRequest)
        {
            var windowFacadeProvider = this.___CreateWindowFacadeProvider(inWindowRequest.CacheWindowObj);
            inWindowRequest.CacheProvider(windowFacadeProvider);
            inWindowRequest.SetStage(WindowRequestStage.FacadeLoading);
            var facade = windowFacadeProvider.Alloc(inViewConfig.ID);
            inWindowRequest.CacheFacade(facade);
            inWindowRequest.SetStage(WindowRequestStage.FacadeLoaded);
            this.__windowSetupCanvas(facade);
            this.__windowSetupRectTransform(facade);
            this.__WindowSetUpLayer(inViewConfig, inWindowRequest.CacheWindowObj, facade);
            inWindowRequest.SetStage(WindowRequestStage.Layer);
            inWindowRequest.SetStage(WindowRequestStage.WindowAwake);
            inWindowRequest.Awake();
            inWindowRequest.SetStage(WindowRequestStage.WindowOpen);
            inWindowRequest.Show();
            return inWindowRequest.CacheWindowObj;
        }

        #endregion

        #region WinodwLife

        private IUIFacadeProvider ___CreateWindowFacadeProvider(Window inWindow)
        {
            var facadeProviderComponent = ViewUtils.CheckAndAdd<UIFacadeProviderDynamic>(inWindow);
            return facadeProviderComponent;
        }

        #endregion
    }
}
