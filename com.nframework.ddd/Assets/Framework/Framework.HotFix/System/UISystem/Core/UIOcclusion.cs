using System.Linq;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// UI遮挡剔除，依赖ViewConfig.IsFullScreen。
    /// 简单理解为：当最上层存在全屏界面时，隐藏它下层所有界面的展示，节省 OverDraw。
    /// 注意时序细节：
    /// 1. 正在播放打开动画的全屏界面还不能参与遮蔽，所以打开动画播完后才调用 OnOpenOcclusion；
    /// 2. 正在关闭的全屏界面必须在关闭动画播放前重新计算遮蔽（把该窗口排除掉），否则下层界面会在动画结束时才突然出现，造成穿帮。
    /// </summary>
    public partial class UISystem
    {
        /// <summary>
        /// 打开完成（有打开动画的话已经播完）后重算一次遮蔽。
        /// </summary>
        /// <param name="inWindowRequest">刚打开的窗口，此时可以正常参与遮蔽计算</param>
        private void OnOpenOcclusion(WindowRequest inWindowRequest)
        {
            this.RefreshOcclusion(null);
        }

        /// <summary>
        /// 开始关闭时立即重算：把正在关闭的窗口排除，因为它马上要播放关闭动画，
        /// 此时下层被它遮蔽的界面要马上恢复，避免动画结束后才突然出现。
        /// </summary>
        /// <param name="inWindowRequest">正在关闭的窗口，本次遮蔽计算中忽略</param>
        private void OnCloseOcclusion(WindowRequest inWindowRequest)
        {
            this.RefreshOcclusion(inWindowRequest);
        }

        /// <summary>
        /// 从最高层向最低层遍历所有已打开的窗口。
        /// 一旦遇到（从前往后看到的）第一个全屏窗口，它后面所有窗口都设为不可见；
        /// 如果没有全屏窗口，则把所有窗口恢复为可见。
        /// </summary>
        /// <param name="inExcludeRequest">需要跳过的窗口请求，一般为正在关闭的请求</param>
        private void RefreshOcclusion(WindowRequest inExcludeRequest)
        {
            if (m_LayerStacks == null || m_LayerStacks.Count == 0)
            {
                return;
            }

            var excludeWindow = inExcludeRequest?.CacheWindowObj;
            var behindFullScreen = false;

            // 层级值越大 sortingOrder 越大，显示越靠前，因此从大到小遍历
            foreach (var layerPair in m_LayerStacks.Reverse())
            {
                var layerStack = layerPair.Value;
                var windows = layerStack.Stack;
                if (windows == null || windows.Count == 0)
                {
                    continue;
                }

                // 同一层级内后 Push 的窗口在最上层，所以从后往前遍历
                for (var i = windows.Count - 1; i >= 0; i--)
                {
                    var window = windows[i];
                    if (window == null || ReferenceEquals(window, excludeWindow))
                    {
                        continue;
                    }

                    if (!this.TryGetWindowRequest(window, out var windowRequest) ||
                        windowRequest == null)
                    {
                        continue;
                    }

                    // WindowOpen/WindowOpenAnim 只是瞬时/中间状态，窗口在动画结束后
                    // 才会置为 WindowOpened 并触发 OnOpenOcclusion，因此只统计 WindowOpened
                    if (windowRequest.Stage != WindowRequestStage.WindowOpened)
                    {
                        continue;
                    }

                    if (behindFullScreen)
                    {
                        // 前面已经存在全屏界面，它后面的窗口全部隐藏
                        this.SetWindowVisible(window, false);
                        continue;
                    }

                    // 尚未遇到全屏界面，窗口保持显示；遇到后其下层全部遮蔽
                    this.SetWindowVisible(window, true);
                    var viewConfig = this.m_ConfigServices?.GetViewConfigByType(window.GetType());
                    if (viewConfig != null && viewConfig.IsFullScreen)
                    {
                        behindFullScreen = true;
                    }
                }
            }
        }

        /// <summary>
        /// 仅在实际状态发生变化时调用 Visible/NotVisible，
        /// 避免每次重算都对所有窗口重复触发显示/隐藏回调。
        /// </summary>
        private void SetWindowVisible(Window inWindow, bool inVisible)
        {
            if (inWindow == null)
            {
                return;
            }

            var facade = inWindow.Facade;
            if (facade == null || facade.gameObject == null)
            {
                return;
            }

            if (inVisible)
            {
                if (!facade.gameObject.activeSelf)
                {
                    inWindow.Visible();
                }
            }
            else
            {
                if (facade.gameObject.activeSelf)
                {
                    inWindow.NotVisible();
                }
            }
        }
    }
}
