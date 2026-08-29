
using Cysharp.Threading.Tasks;

namespace NFramework.ModuleSystem
{
    public partial class UISystem
    {
        public UniTask OpenAsync<T>() where T : Window, new()
        {
            var vc = this.GetViewConfig<T>();
            return _OpenAsync<T>(vc);
        }
        public UniTask OpenAsync<T, I>(I inViewData) where T : Window, IViewSetData<I>, new() where I : class
        {
            var vc = this.GetViewConfig<T>();
            return _OpenAsync<T, I>(vc, inViewData);
        }
        public UniTask OpenAsync(string inWindowName)
        {
            var vc = this.GetViewConfig(inWindowName);
            return _OpenAsync(vc);
        }
        public UniTask OpenAsync<I>(string inWindowName, I inViewData = null) where I : class
        {
            var vc = this.GetViewConfig(inWindowName);
            return _OpenAsync<I>(vc, inViewData);
        }

        
        public Window OpenSync<T>() where T : Window, new()
        {
            var vc = this.GetViewConfig<T>();
            return _OpenSync<T>(vc);
        }
        public Window OpenSync<T, I>(I inViewData) where T : Window, IViewSetData<I>, new() where I : class
        {
            var vc = this.GetViewConfig<T>();
            return _OpenSync<T, I>(vc, inViewData);
        }

        public Window OpenSync(string inWindowName)
        {
            var vc = this.GetViewConfig(inWindowName);
            return _OpenSync(vc);
        }
        public Window OpenSync<I>(string inWindowName, I inViewData = null) where I : class
        {
            var vc = this.GetViewConfig(inWindowName);
            return _OpenSync<I>(vc, inViewData);
        }   

    }
}
