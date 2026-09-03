using System.Collections.Generic;

namespace NFramework.ModuleSystem
{
    public partial class UISystem
    {
        /// <summary>关闭某个类型的默认单例实例，或指定 key 的实例</summary>
        public void Close<T>(string key = null) where T : View
        {
            this._Close(this.GetViewID<T>(), key);
        }

        /// <summary>直接关闭指定窗口实例</summary>
        public void Close<T>(T inWindow) where T : Window
        {
            this._Close(inWindow);
        }

        /// <summary>直接关闭指定窗口实例</summary>
        public void Close(Window inWindow)
        {
            this._Close(inWindow);
        }

        /// <summary>按窗口ID + key 关闭，key 不传默认关单例实例</summary>
        public void Close(string inWindowID, string key = null)
        {
            this._Close(inWindowID, key);
        }

        /// <summary>关闭某个 UILayer 上所有打开的窗口</summary>
        public void CloseLayer(UILayer inLayer)
        {
            this.CloseLayer((ushort)inLayer);
        }

        /// <summary>按层级值关闭该层所有打开的窗口（Layer 为 0 时按 Basic 处理）</summary>
        public void CloseLayer(ushort inLayer)
        {
            var targets = new List<WindowRequest>();
            foreach (var pair in WindowRequestDictionary)
            {
                if (IsRequestOnLayer(pair.Value, inLayer))
                {
                    targets.Add(pair.Value);
                }
            }

            foreach (var request in targets)
            {
                this.CloseRequest(request);
            }
        }

        /// <summary>关闭 UISystem 当前管理的所有窗口，并清空窗口池中的缓存</summary>
        public void CloseAllWindows()
        {
            var allRequests = new List<WindowRequest>(WindowRequestDictionary.Values);
            foreach (var request in allRequests)
            {
                if (this.TryGetWindowRequest(request.RequestKey, out var current) &&
                    ReferenceEquals(current, request))
                {
                    this.CloseRequest(request);
                }
            }

            this.m_Pool?.ClearAll();
        }

        private static bool IsRequestOnLayer(WindowRequest inRequest, ushort inLayer)
        {
            if (inRequest == null || inRequest.Config == null)
            {
                return false;
            }

            ushort requestLayer = inRequest.Config.Layer == 0
                ? (ushort)UILayer.Basic
                : inRequest.Config.Layer;
            ushort targetLayer = inLayer == 0 ? (ushort)UILayer.Basic : inLayer;
            return requestLayer == targetLayer;
        }
    }
}
