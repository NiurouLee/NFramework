using Cysharp.Threading.Tasks;

namespace NFramework.ModuleSystem
{
    public partial class UISystem
    {
        /// <summary>key 为空表示默认单例实例；传入不同 key 可同时打开同类型的多个窗口</summary>
        public UniTask OpenAsync<T>(string key = null) where T : Window, new()
        {
            var vc = this.GetViewConfig<T>();
            return _OpenAsync<T>(vc, key);
        }

        /// <summary>key 为空表示默认单例实例；传入不同 key 可同时打开同类型的多个窗口</summary>
        public UniTask OpenAsync<T, I>(I inViewData, string key = null)
            where T : Window, IViewSetData<I>, new() where I : class
        {
            var vc = this.GetViewConfig<T>();
            return _OpenAsync<T, I>(vc, inViewData, key);
        }

        /// <summary>key 为空表示默认单例实例；传入不同 key 可同时打开同类型的多个窗口</summary>
        public UniTask OpenAsync(string inWindowName, string key = null)
        {
            var vc = this.GetViewConfig(inWindowName);
            return _OpenAsync(vc, key);
        }

        /// <summary>key 为空表示默认单例实例；传入不同 key 可同时打开同类型的多个窗口</summary>
        public UniTask OpenAsync<I>(string inWindowName, I inViewData = null, string key = null) where I : class
        {
            var vc = this.GetViewConfig(inWindowName);
            return _OpenAsync<I>(vc, inViewData, key);
        }

        /// <summary>key 为空表示默认单例实例；传入不同 key 可同时打开同类型的多个窗口</summary>
        public Window OpenSync<T>(string key = null) where T : Window, new()
        {
            var vc = this.GetViewConfig<T>();
            return _OpenSync<T>(vc, key);
        }

        /// <summary>key 为空表示默认单例实例；传入不同 key 可同时打开同类型的多个窗口</summary>
        public Window OpenSync<T, I>(I inViewData, string key = null)
            where T : Window, IViewSetData<I>, new() where I : class
        {
            var vc = this.GetViewConfig<T>();
            return _OpenSync<T, I>(vc, inViewData, key);
        }

        /// <summary>key 为空表示默认单例实例；传入不同 key 可同时打开同类型的多个窗口</summary>
        public Window OpenSync(string inWindowName, string key = null)
        {
            var vc = this.GetViewConfig(inWindowName);
            return _OpenSync(vc, key);
        }

        /// <summary>key 为空表示默认单例实例；传入不同 key 可同时打开同类型的多个窗口</summary>
        public Window OpenSync<I>(string inWindowName, I inViewData = null, string key = null) where I : class
        {
            var vc = this.GetViewConfig(inWindowName);
            return _OpenSync<I>(vc, inViewData, key);
        }

        /// <summary>判断某个类型窗口的默认/指定 key 实例是否处于打开状态</summary>
        public bool IsWindowOpen<T>(string key = null) where T : View
        {
            return this.IsWindowOpen(this.GetViewID<T>(), key);
        }

        /// <summary>按窗口ID + key 判断是否处于打开状态</summary>
        public bool IsWindowOpen(string inWindowID, string key = null)
        {
            if (string.IsNullOrEmpty(inWindowID))
            {
                return false;
            }

            var requestKey = WindowRequest.MakeRequestKey(inWindowID, key);
            return this.TryGetWindowRequest(requestKey, out var request) && IsRequestOpen(request);
        }

        /// <summary>直接判断指定窗口实例是否处于打开状态</summary>
        public bool IsWindowOpen(Window inWindow)
        {
            return this.TryGetWindowRequest(inWindow, out var request) && IsRequestOpen(request);
        }

        /// <summary>某个 UILayer 对应的 Stack 是否为空</summary>
        public bool IsLayerEmpty(UILayer inLayer)
        {
            return this.IsLayerEmpty((ushort)inLayer);
        }

        /// <summary>按层级值判断对应 Stack 是否为空（Layer 为 0 时按 Basic 处理）</summary>
        public bool IsLayerEmpty(ushort inLayer)
        {
            if (this.TryGetLayerStack(inLayer, out var layerStack))
            {
                return layerStack.IsEmpty;
            }

            return true;
        }

        private static bool IsRequestOpen(WindowRequest inRequest)
        {
            return inRequest != null &&
                   (inRequest.Stage == WindowRequestStage.WindowOpen ||
                    inRequest.Stage == WindowRequestStage.WindowOpenAnim ||
                    inRequest.Stage == WindowRequestStage.WindowOpened);
        }
    }
}
