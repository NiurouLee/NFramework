using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    public partial class UISystem
    {
        /// <summary>按窗口ID + key 关闭，key 不传默认关单例实例</summary>
        private void _Close(string inWindowName, string inKey = null)
        {
            if (string.IsNullOrEmpty(inWindowName))
            {
                this.GetSystem<LoggerSystem>()?.ErrStack("UIM::Close inWindowName is null");
                return;
            }

            var requestKey = WindowRequest.MakeRequestKey(inWindowName, inKey);
            if (this.TryGetWindowRequest(requestKey, out var outWindowRequest))
            {
                this.CloseRequest(outWindowRequest);
            }
        }

        /// <summary>直接关闭指定窗口实例，适合同类型打开多个实例时由 window.Close() 调用</summary>
        private void _Close(Window inWindow)
        {
            if (inWindow == null)
            {
                this.GetSystem<LoggerSystem>()?.ErrStack("UIM::Close inWindow is null");
                return;
            }

            if (this.TryGetWindowRequest(inWindow, out var outWindowRequest))
            {
                this.CloseRequest(outWindowRequest);
            }
            else
            {
                this.GetSystem<LoggerSystem>()
                    ?.ErrStack($"UIM::Close window not in WindowRequestDictionary, WindowName:{inWindow.GetType().Name}");
            }
        }

        private void CloseRequest(WindowRequest inWindowRequest)
        {
            if (inWindowRequest == null)
            {
                return;
            }

            switch (inWindowRequest.Stage)
            {
                case WindowRequestStage.WindowOpen:
                    this.StartCloseAsync(inWindowRequest);
                    break;
                case WindowRequestStage.WindowOpenAnim:
                    // 正在播放打开动画时收到关闭：取消打开，走半成品清理
                    inWindowRequest.MarkCanceled();
                    this.CancelRequest(inWindowRequest);
                    break;
                case WindowRequestStage.FacadeLoading:
                    this.CancelRequest(inWindowRequest);
                    break;
                default:
                    this.GetSystem<LoggerSystem>()?.ErrStack(
                        $"UIM::Close ignored, WindowName:{inWindowRequest.Name}, Key:{inWindowRequest.keyObj}, Stage:{inWindowRequest.Stage}");
                    break;
            }
        }

        private void StartCloseAsync(WindowRequest inWindowRequest)
        {
            if (inWindowRequest == null)
            {
                return;
            }

            inWindowRequest.EnableCancellation();
            CloseWithAnimationAsync(inWindowRequest).Forget();
        }

        private async UniTaskVoid CloseWithAnimationAsync(WindowRequest inWindowRequest)
        {
            var window = inWindowRequest.CacheWindowObj;
            inWindowRequest.SetStage(WindowRequestStage.WindowClose);
            try
            {
                if (window != null)
                {
                    await window.PlayCloseAnimationAsync(inWindowRequest.CancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (inWindowRequest.IsCanceled)
            {
                return;
            }

            inWindowRequest.SetStage(WindowRequestStage.WindowCloseAnim);
            this.__Close(inWindowRequest);
        }

        /// <summary>
        /// 正常关闭：出层、归还 order、从请求表移除后入 LRU 窗口池。
        /// 池容量满时由 LRU 淘汰最久未使用的窗口，淘汰项会真正销毁。
        /// </summary>
        private void __Close(WindowRequest inWindowRequest)
        {
            var inWindow = inWindowRequest.CacheWindowObj;
            if (inWindow == null)
            {
                this.RemoveWindowRequest(inWindowRequest.RequestKey);
                return;
            }

            inWindow.Hide();
            if (inWindowRequest.Config != null &&
                this.TryGetLayerStack(inWindowRequest.Config.Layer, out var layerStack))
            {
                layerStack.PopWindow(inWindow, inWindowRequest.CacheOrderObj);
            }
            else
            {
                this.GetSystem<LoggerSystem>()
                    ?.ErrStack($"UIM::Close can not find layer stack, WindowID:{inWindowRequest.Name}, " +
                               $"Layer:{inWindowRequest.Config?.Layer}");
            }

            this.RemoveWindowRequest(inWindowRequest.RequestKey);
            if (this.WindowPoolEnabled)
            {
                this.CacheWindowToPool(inWindowRequest);
            }
            else
            {
                this.DestroyWindowObject(inWindow);
            }
        }

        /// <summary>把窗口收进 LRU 池；没有完整 Facade 时无法复用，直接销毁</summary>
        private void CacheWindowToPool(WindowRequest inWindowRequest)
        {
            var window = inWindowRequest.CacheWindowObj;
            var facade = inWindowRequest.CacheFacadeObj ?? window?.Facade;
            if (window == null || facade == null)
            {
                if (facade != null && facade.gameObject != null)
                {
                    Object.Destroy(facade.gameObject);
                }

                window?.Destroy();
                return;
            }

            // 回池时不改父节点：窗口已从层逻辑移除并 Hide，留在原地即可，下次复用再挂回同层
            this.m_Pool.Cache(inWindowRequest.RequestKey, window);
        }

        /// <summary>加载中取消/失败：移除请求、归还 order、销毁半成品窗口</summary>
        private void CancelRequest(WindowRequest inWindowRequest)
        {
            if (inWindowRequest == null)
            {
                return;
            }

            inWindowRequest.MarkCanceled();
            this.RemoveWindowRequest(inWindowRequest.RequestKey);
            this.TryPopOrReturnLayer(inWindowRequest);
            inWindowRequest.Deferred?.TrySetCanceled();

            var window = inWindowRequest.CacheWindowObj;
            var facade = inWindowRequest.CacheFacadeObj ?? window?.Facade;
            var facadeGo = facade != null && facade.gameObject != null ? facade.gameObject : null;
            window?.Destroy();
            if (facadeGo != null)
            {
                Object.Destroy(facadeGo);
            }
        }

        /// <summary>异步加载/打开过程出现异常时统一清理（不会入池）</summary>
        internal void CleanupFailedRequest(WindowRequest inWindowRequest, UIFacade inLoadFacade)
        {
            if (inWindowRequest == null)
            {
                return;
            }

            this.RemoveWindowRequest(inWindowRequest.RequestKey);
            this.TryPopOrReturnLayer(inWindowRequest);
            inWindowRequest.Deferred?.TrySetCanceled();

            var window = inWindowRequest.CacheWindowObj;
            var facade = inLoadFacade ?? inWindowRequest.CacheFacadeObj ?? window?.Facade;
            var facadeGo = facade != null && facade.gameObject != null ? facade.gameObject : null;
            window?.Destroy();
            if (facadeGo != null)
            {
                Object.Destroy(facadeGo);
            }
        }

        /// <summary>请求异常清理：已入层则从层移除并归还 order，否则只归还 order</summary>
        private void TryPopOrReturnLayer(WindowRequest inWindowRequest)
        {
            if (inWindowRequest == null || inWindowRequest.Config == null)
            {
                return;
            }

            if (this.TryGetLayerStack(inWindowRequest.Config.Layer, out var layerStack))
            {
                var window = inWindowRequest.CacheWindowObj;
                if (window != null && layerStack.Contains(window))
                {
                    layerStack.PopWindow(window, inWindowRequest.CacheOrderObj);
                }
                else
                {
                    layerStack.ReturnOrder(inWindowRequest.CacheOrderObj);
                }
            }
        }

        /// <summary>真正销毁一个不再进入池的窗口（池淘汰 / 异常兜底）</summary>
        internal void DestroyWindowObject(Window inWindow)
        {
            if (inWindow == null)
            {
                return;
            }

            var facade = inWindow.Facade;
            var facadeGo = facade != null && facade.gameObject != null ? facade.gameObject : null;
            inWindow.Destroy();
            if (facadeGo != null)
            {
                Object.Destroy(facadeGo);
            }
        }
    }
}
