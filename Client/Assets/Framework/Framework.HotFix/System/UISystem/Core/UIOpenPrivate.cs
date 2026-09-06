using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace NFramework.ModuleSystem
{
    public partial class UISystem
    {
        #region Cache

        /// <summary>
        /// 打开中、打开的 WindowRequest。
        /// key 为 RequestKey（窗口ID + Key），因此同一 ViewConfig 配不同 Key 可以并存多个实例。
        /// </summary>
        /// <summary>内部请求表，外部只应通过 Open/Close API 操作</summary>
        internal Dictionary<string, WindowRequest> WindowRequestDictionary = new Dictionary<string, WindowRequest>();

        private bool TryGetWindowRequest(string inWindowRequestKey, out WindowRequest outWindowRequest)
        {
            return this.WindowRequestDictionary.TryGetValue(inWindowRequestKey, out outWindowRequest);
        }

        private bool CheckWindowReq(ViewConfig inViewConfig, string inKey, out WindowRequest outWindowRequest)
        {
            return TryGetWindowRequest(WindowRequest.MakeRequestKey(inViewConfig.ID, inKey), out outWindowRequest);
        }

        private bool TryGetWindowRequest(Window inWindow, out WindowRequest outWindowRequest)
        {
            foreach (var pair in WindowRequestDictionary)
            {
                if (ReferenceEquals(pair.Value.CacheWindowObj, inWindow))
                {
                    outWindowRequest = pair.Value;
                    return true;
                }
            }

            outWindowRequest = null;
            return false;
        }

        #endregion

        private void AddWindowRequest(WindowRequest inWindowRequest)
        {
            WindowRequestDictionary.Add(inWindowRequest.RequestKey, inWindowRequest);
        }

        private bool RemoveWindowRequest(string inWindowRequestKey)
        {
            return WindowRequestDictionary.Remove(inWindowRequestKey);
        }

        #region common

        private WindowRequest CreateRequestAndWindow<TW>(ViewConfig inViewConfig, string inKey = null)
            where TW : Window, new()
        {
            var windowRequest = CreateRequest<TW>(inViewConfig, inKey);
            var window = this.CreateView<TW>();
            windowRequest.CacheWindow(window);
            windowRequest.SetStage(WindowRequestStage.Cache);
            return windowRequest;
        }

        private WindowRequestByWindow<TW> CreateRequest<TW>(ViewConfig inViewConfig, string inKey = null)
            where TW : Window
        {
            var windowRequest = new WindowRequestByWindow<TW>(inViewConfig);
            windowRequest.CacheKey(inKey);
            AddWindowRequest(windowRequest);
            windowRequest.SetStage(WindowRequestStage.Construct);
            return windowRequest;
        }

        private WindowRequest CreateRequestAndWindow<TW, TD>(ViewConfig inViewConfig, TD inViewData,
            string inKey = null)
            where TW : Window, IViewSetData<TD>, new() where TD : class
        {
            var windowRequest = CreateRequest<TW, TD>(inViewConfig, inViewData, inKey);
            var window = this.CreateView<TW>();
            windowRequest.CacheWindowAndData(window, inViewData);
            windowRequest.SetStage(WindowRequestStage.Cache);
            return windowRequest;
        }

        private WindowRequest<TW, TD> CreateRequest<TW, TD>(ViewConfig inViewConfig, TD inViewData, string inKey = null)
            where TW : Window, IViewSetData<TD>, new() where TD : class
        {
            var windowRequest = new WindowRequest<TW, TD>(inViewConfig);
            windowRequest.CacheKey(inKey);
            AddWindowRequest(windowRequest);
            windowRequest.SetStage(WindowRequestStage.Construct);
            return windowRequest;
        }

        private WindowRequestByWindow CreateRequest(ViewConfig inViewConfig, string inKey = null)
        {
            var windowRequest = new WindowRequestByWindow(inViewConfig);
            windowRequest.CacheKey(inKey);
            AddWindowRequest(windowRequest);
            windowRequest.SetStage(WindowRequestStage.Construct);
            var window = this.CreateView(inViewConfig) as Window;
            windowRequest.Setup(window);
            windowRequest.SetStage(WindowRequestStage.Cache);
            return windowRequest;
        }

        /// <summary>仅创建 WindowRequestByData，窗口由调用方（例如复用池）再 Cache</summary>
        private WindowRequestByData<TD> CreateDataRequest<TD>(ViewConfig inViewConfig, string inKey = null)
            where TD : class
        {
            var windowRequest = new WindowRequestByData<TD>(inViewConfig);
            windowRequest.CacheKey(inKey);
            AddWindowRequest(windowRequest);
            windowRequest.SetStage(WindowRequestStage.Construct);
            return windowRequest;
        }

        private WindowRequestByData<TD> CreateRequestByData<TD>(ViewConfig inViewConfig, TD inViewData,
            string inKey = null) where TD : class
        {
            var windowRequest = CreateDataRequest<TD>(inViewConfig, inKey);
            var window = this.CreateView(inViewConfig) as Window;
            windowRequest.CacheWindowAndData(window, inViewData);
            windowRequest.SetStage(WindowRequestStage.Cache);
            return windowRequest;
        }

        private void AllocOrder(ViewConfig inViewConfig, WindowRequest windowRequest)
        {
            if (this.TryGetLayerStack(inViewConfig.Layer, out var layerStack))
            {
                windowRequest.CacheOrder(layerStack.GetOrder());
            }
        }

        private bool TryGetPooledWindow<T>(string inRequestKey, out T outWindow) where T : Window
        {
            outWindow = null;
            return this.WindowPoolEnabled && this.m_Pool != null &&
                   this.m_Pool.TryGetWindow(inRequestKey, out outWindow);
        }

        private bool TryGetPooledWindow(string inRequestKey, out Window outWindow)
        {
            outWindow = null;
            return this.WindowPoolEnabled && this.m_Pool != null &&
                   this.m_Pool.TryGetWindow(inRequestKey, out outWindow);
        }

        /// <summary>
        /// 复用池窗口时把已有 Window/Facade/Provider 绑定到新请求上。
        /// 窗口已经 Awake，因此之后不会再调 request.Awake()。
        /// </summary>
        private void CacheWindowFromPool(WindowRequest inWindowRequest, Window inWindow)
        {
            if (inWindow == null)
            {
                throw new Exception("CacheWindowFromPool window is null");
            }

            if (inWindow.Facade == null || inWindow.Provider == null)
            {
                throw new Exception($"CacheWindowFromPool window not complete, ID:{inWindowRequest.Name}");
            }

            inWindowRequest.CacheWindow(inWindow);
            inWindowRequest.CacheFacade(inWindow.Facade);
            inWindowRequest.CacheProvider(inWindow.Provider);
            inWindowRequest.SetStage(WindowRequestStage.Cache);
        }

        /// <summary>从池复用：同步把窗口挂回层级并 Show（异步调用返回已完成 task）</summary>
        private async UniTask OpenFromPoolAsync(ViewConfig inViewConfig, WindowRequest inWindowRequest, Window inWindow)
        {
            var deferred = new UniTaskCompletionSource();
            inWindowRequest.SetTaskCompletionSource(deferred);

            try
            {
                this.CacheWindowFromPool(inWindowRequest, inWindow);
                this.AllocOrder(inViewConfig, inWindowRequest);
                inWindowRequest.SetStage(WindowRequestStage.Layer);
                this.__WindowSetUpLayer(inViewConfig, inWindowRequest.CacheWindowObj,
                    inWindowRequest.CacheFacadeObj, inWindowRequest.CacheOrderObj);

                inWindowRequest.PrepareReuse();
                inWindowRequest.SetStage(WindowRequestStage.WindowOpen);
                inWindowRequest.Show();
                deferred.TrySetResult();
                this.StartPlayOpenAnimAndOcclusion(inWindowRequest);
            }
            catch (Exception e)
            {
                this.GetSystem<LoggerSystem>()
                    ?.ErrStack($"Open window from pool failed, WindowName:{inWindowRequest.Name}, Error:{e}");
                this.CleanupFailedRequest(inWindowRequest, inWindowRequest.CacheFacadeObj);
            }

            return;
        }

        /// <summary>从池复用（同步 API）</summary>
        private Window OpenFromPoolSync(ViewConfig inViewConfig, WindowRequest inWindowRequest, Window inWindow)
        {
            try
            {
                this.CacheWindowFromPool(inWindowRequest, inWindow);
                this.OpenPooledWindow(inViewConfig, inWindowRequest);
                return inWindowRequest.CacheWindowObj;
            }
            catch (Exception e)
            {
                this.GetSystem<LoggerSystem>()
                    ?.ErrStack($"Open window from pool failed, WindowName:{inWindowRequest.Name}, Error:{e}");
                this.CleanupFailedRequest(inWindowRequest, inWindowRequest.CacheFacadeObj);
                return null;
            }
        }

        private void OpenPooledWindow(ViewConfig inViewConfig, WindowRequest inWindowRequest)
        {
            this.AllocOrder(inViewConfig, inWindowRequest);
            inWindowRequest.SetStage(WindowRequestStage.Layer);
            this.__WindowSetUpLayer(inViewConfig, inWindowRequest.CacheWindowObj, inWindowRequest.CacheFacadeObj,
                inWindowRequest.CacheOrderObj);
            inWindowRequest.SetStage(WindowRequestStage.WindowOpen);
            inWindowRequest.PrepareReuse();
            inWindowRequest.Show();
            this.StartPlayOpenAnimAndOcclusion(inWindowRequest);
        }

        #endregion

        private async void StartPlayOpenAnimAndOcclusion(WindowRequest inWindowRequest)
        {
            if (this.HaveOpenAnim(inWindowRequest, out var time))
            {
                // 打开动画期间全屏窗口还不参与遮蔽，播完置为完全打开后再重算
                inWindowRequest.SetStage(WindowRequestStage.WindowOpenAnim);
                this.PlayOpenAnim(inWindowRequest.Config, inWindowRequest);
                await UniTask.WaitForSeconds(time);
                if (inWindowRequest.IsCanceled)
                {
                    return;
                }

                inWindowRequest.SetStage(WindowRequestStage.WindowOpened);
                this.OnOpenOcclusion(inWindowRequest);
            }
            else
            {
                inWindowRequest.SetStage(WindowRequestStage.WindowOpened);
                this.OnOpenOcclusion(inWindowRequest);
            }
        }
        

        #region Async

        private UniTask _OpenAsync<TW>(ViewConfig inViewConfig, string inKey = null) where TW : Window, new()
        {
            // 同一个 ViewConfig + Key 已经打开/正在打开时直接复用它的 task
            if (CheckWindowReq(inViewConfig, inKey, out var outWindowRequest))
            {
                return outWindowRequest.Deferred.Task;
            }

            var requestKey = WindowRequest.MakeRequestKey(inViewConfig.ID, inKey);
            if (this.TryGetPooledWindow<TW>(requestKey, out var window))
            {
                var pooledRequest = CreateRequest<TW>(inViewConfig, inKey);
                return OpenFromPoolAsync(inViewConfig, pooledRequest, window);
            }

            var windowRequest = CreateRequestAndWindow<TW>(inViewConfig, inKey);
            AllocOrder(inViewConfig, windowRequest);
            var deferred = new UniTaskCompletionSource();
            windowRequest.SetTaskCompletionSource(deferred);
            ___AsyncSetFacade(inViewConfig, windowRequest, deferred).Forget();
            return deferred.Task;
        }

        private UniTask _OpenAsync<TD>(ViewConfig inViewConfig, TD inViewData, string inKey = null) where TD : class
        {
            if (CheckWindowReq(inViewConfig, inKey, out var outWindowRequest))
            {
                return outWindowRequest.Deferred.Task;
            }

            var requestKey = WindowRequest.MakeRequestKey(inViewConfig.ID, inKey);
            if (this.TryGetPooledWindow(requestKey, out var window))
            {
                var pooledRequest = CreateDataRequest<TD>(inViewConfig, inKey);
                if (inViewData != null)
                {
                    pooledRequest.CacheViewData(inViewData);
                }

                return OpenFromPoolAsync(inViewConfig, pooledRequest, window);
            }

            var windowRequest = CreateRequestByData<TD>(inViewConfig, inViewData, inKey);
            AllocOrder(inViewConfig, windowRequest);
            var deferred = new UniTaskCompletionSource();
            windowRequest.SetTaskCompletionSource(deferred);
            ___AsyncSetFacade(inViewConfig, windowRequest, deferred).Forget();
            return deferred.Task;
        }

        private UniTask _OpenAsync<TW, TD>(ViewConfig inViewConfig, TD inViewData, string inKey = null)
            where TW : Window, IViewSetData<TD>, new() where TD : class
        {
            if (CheckWindowReq(inViewConfig, inKey, out var outWindowRequest))
            {
                return outWindowRequest.Deferred.Task;
            }

            var requestKey = WindowRequest.MakeRequestKey(inViewConfig.ID, inKey);
            if (this.TryGetPooledWindow<TW>(requestKey, out var window))
            {
                var pooledRequest = CreateRequest<TW, TD>(inViewConfig, inViewData, inKey);
                if (inViewData != null)
                {
                    pooledRequest.CacheViewData(inViewData);
                }

                return OpenFromPoolAsync(inViewConfig, pooledRequest, window);
            }

            var windowRequest = CreateRequestAndWindow<TW, TD>(inViewConfig, inViewData, inKey);
            AllocOrder(inViewConfig, windowRequest);
            var deferred = new UniTaskCompletionSource();
            windowRequest.SetTaskCompletionSource(deferred);
            ___AsyncSetFacade(inViewConfig, windowRequest, deferred).Forget();
            return deferred.Task;
        }

        private UniTask _OpenAsync(ViewConfig inViewConfig, string inKey = null)
        {
            if (CheckWindowReq(inViewConfig, inKey, out var outWindowRequest))
            {
                return outWindowRequest.Deferred.Task;
            }

            var requestKey = WindowRequest.MakeRequestKey(inViewConfig.ID, inKey);
            if (this.TryGetPooledWindow(requestKey, out var window))
            {
                var pooledRequest = CreateRequest(inViewConfig, inKey);
                return OpenFromPoolAsync(inViewConfig, pooledRequest, window);
            }

            WindowRequestByWindow windowRequest = CreateRequest(inViewConfig, inKey);
            AllocOrder(inViewConfig, windowRequest);
            var deferred = new UniTaskCompletionSource();
            windowRequest.SetTaskCompletionSource(deferred);
            ___AsyncSetFacade(inViewConfig, windowRequest, deferred).Forget();
            return deferred.Task;
        }

        private async UniTaskVoid ___AsyncSetFacade(ViewConfig inViewConfig, WindowRequest inWindowRequest,
            UniTaskCompletionSource inDeferred)
        {
            UIFacade loadFacade = null;
            try
            {
                var windowFacadeProvider = this.___CreateWindowFacadeProvider(inWindowRequest.CacheWindowObj);
                inWindowRequest.CacheProvider(windowFacadeProvider);
                inWindowRequest.SetStage(WindowRequestStage.FacadeLoading);

                inWindowRequest.EnableCancellation();
                loadFacade = await windowFacadeProvider.AllocAsync(inViewConfig.ID, inWindowRequest.CancellationToken)
                    .Task;
                if (inWindowRequest.IsCanceled)
                {
                    if (loadFacade != null && loadFacade.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(loadFacade.gameObject);
                    }

                    return;
                }

                inWindowRequest.CacheFacade(loadFacade);
                inWindowRequest.SetStage(WindowRequestStage.FacadeLoaded);
                this.__windowSetupCanvas(loadFacade);
                inWindowRequest.SetStage(WindowRequestStage.Layer);
                this.__WindowSetUpLayer(inViewConfig, inWindowRequest.CacheWindowObj, loadFacade,
                    inWindowRequest.CacheOrderObj);
                this.__windowSetupRectTransform(loadFacade);
                inWindowRequest.SetStage(WindowRequestStage.WindowAwake);
                inWindowRequest.Awake();
                inWindowRequest.Show();
                inWindowRequest.SetStage(WindowRequestStage.WindowOpen);
                this.StartPlayOpenAnimAndOcclusion(inWindowRequest);
                inDeferred.TrySetResult();
            }
            catch (OperationCanceledException)
            {
                this.CleanupFailedRequest(inWindowRequest, loadFacade);
            }
            catch (Exception e)
            {
                this.GetSystem<LoggerSystem>()
                    ?.ErrStack($"OpenWindow facade load failed, WindowName:{inWindowRequest.Name}, Error:{e}");
                this.CleanupFailedRequest(inWindowRequest, loadFacade);
            }
        }

        #endregion

        #region Sync

        private TW _OpenSync<TW>(ViewConfig inViewConfig, string inKey = null) where TW : Window, new()
        {
            if (CheckWindowReq(inViewConfig, inKey, out var outWindowRequest))
            {
                return outWindowRequest.CacheWindowObj as TW;
            }

            var requestKey = WindowRequest.MakeRequestKey(inViewConfig.ID, inKey);
            if (this.TryGetPooledWindow<TW>(requestKey, out var pooledWindow))
            {
                var pooledRequest = CreateRequest<TW>(inViewConfig, inKey);
                return OpenFromPoolSync(inViewConfig, pooledRequest, pooledWindow) as TW;
            }

            var windowRequest = CreateRequestAndWindow<TW>(inViewConfig, inKey);
            AllocOrder(inViewConfig, windowRequest);
            return ___SyncSetFacade(inViewConfig, windowRequest) as TW;
        }

        private Window _OpenSync<TD>(ViewConfig inViewConfig, TD inViewData, string inKey = null) where TD : class
        {
            if (CheckWindowReq(inViewConfig, inKey, out var outWindowRequest))
            {
                return outWindowRequest.CacheWindowObj;
            }

            var requestKey = WindowRequest.MakeRequestKey(inViewConfig.ID, inKey);
            if (this.TryGetPooledWindow(requestKey, out var pooledWindow))
            {
                var pooledRequest = CreateDataRequest<TD>(inViewConfig, inKey);
                if (inViewData != null)
                {
                    pooledRequest.CacheViewData(inViewData);
                }

                return OpenFromPoolSync(inViewConfig, pooledRequest, pooledWindow);
            }

            var windowRequest = CreateRequestByData<TD>(inViewConfig, inViewData, inKey);
            AllocOrder(inViewConfig, windowRequest);
            return ___SyncSetFacade(inViewConfig, windowRequest);
        }

        private TW _OpenSync<TW, TD>(ViewConfig inViewConfig, TD inViewData, string inKey = null)
            where TW : Window, IViewSetData<TD>, new() where TD : class
        {
            if (CheckWindowReq(inViewConfig, inKey, out var outWindowRequest))
            {
                return outWindowRequest.CacheWindowObj as TW;
            }

            var requestKey = WindowRequest.MakeRequestKey(inViewConfig.ID, inKey);
            if (this.TryGetPooledWindow<TW>(requestKey, out var pooledWindow))
            {
                var pooledRequest = CreateRequest<TW, TD>(inViewConfig, inViewData, inKey);
                if (inViewData != null)
                {
                    pooledRequest.CacheViewData(inViewData);
                }

                return OpenFromPoolSync(inViewConfig, pooledRequest, pooledWindow) as TW;
            }

            var windowRequest = CreateRequestAndWindow<TW, TD>(inViewConfig, inViewData, inKey);
            AllocOrder(inViewConfig, windowRequest);
            return ___SyncSetFacade(inViewConfig, windowRequest) as TW;
        }

        private Window _OpenSync(ViewConfig inViewConfig, string inKey = null)
        {
            if (CheckWindowReq(inViewConfig, inKey, out var outWindowRequest))
            {
                return outWindowRequest.CacheWindowObj;
            }

            var requestKey = WindowRequest.MakeRequestKey(inViewConfig.ID, inKey);
            if (this.TryGetPooledWindow(requestKey, out var pooledWindow))
            {
                var pooledRequest = CreateRequest(inViewConfig, inKey);
                return OpenFromPoolSync(inViewConfig, pooledRequest, pooledWindow);
            }

            var windowRequest = CreateRequest(inViewConfig, inKey);
            AllocOrder(inViewConfig, windowRequest);
            return ___SyncSetFacade(inViewConfig, windowRequest);
        }

        private Window ___SyncSetFacade(ViewConfig inViewConfig, WindowRequest inWindowRequest)
        {
            try
            {
                var windowFacadeProvider = this.___CreateWindowFacadeProvider(inWindowRequest.CacheWindowObj);
                inWindowRequest.CacheProvider(windowFacadeProvider);
                inWindowRequest.SetStage(WindowRequestStage.FacadeLoading);
                var facade = windowFacadeProvider.Alloc(inViewConfig.ID);
                inWindowRequest.CacheFacade(facade);
                inWindowRequest.SetStage(WindowRequestStage.FacadeLoaded);
                this.__windowSetupCanvas(facade);
                this.__windowSetupRectTransform(facade);
                inWindowRequest.SetStage(WindowRequestStage.Layer);
                this.__WindowSetUpLayer(inViewConfig, inWindowRequest.CacheWindowObj, facade,
                    inWindowRequest.CacheOrderObj);
                inWindowRequest.SetStage(WindowRequestStage.WindowAwake);
                inWindowRequest.Awake();
                inWindowRequest.SetStage(WindowRequestStage.WindowOpen);
                inWindowRequest.Show();
                this.StartPlayOpenAnimAndOcclusion(inWindowRequest);
                return inWindowRequest.CacheWindowObj;
            }
            catch (Exception e)
            {
                this.GetSystem<LoggerSystem>()
                    ?.ErrStack($"OpenWindow sync failed, WindowName:{inWindowRequest.Name}, Error:{e}");
                this.CleanupFailedRequest(inWindowRequest, inWindowRequest.CacheFacadeObj);
                return null;
            }
        }

        #endregion

        #region WindowLife

        private IUIFacadeProvider ___CreateWindowFacadeProvider(Window inWindow)
        {
            var facadeProviderComponent = ViewUtils.CheckAndAdd<UIFacadeProviderDynamic>(inWindow);
            return facadeProviderComponent;
        }

        #endregion
    }
}
