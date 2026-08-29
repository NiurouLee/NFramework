
using Cysharp.Threading.Tasks;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 带池的provider
    /// </summary>
    public class UIFacadeProviderPool : IUIFacadeProvider
    {
        public UIFacade Alloc<T>() where T : View
        {
            throw new System.NotImplementedException();
        }

        public UniTaskCompletionSource<UIFacade> AllocAsync<T>() where T : View
        {
            throw new System.NotImplementedException();
        }

        public UniTaskCompletionSource<UIFacade> AllocAsync(string inViewID)
        {
            throw new System.NotImplementedException();
        }

        public UIFacade Alloc(string inViewID)
        {
            return null;
        }

        public void Destroy()
        {
        }

        public void Free(UIFacade inUIFacade)
        {
            throw new System.NotImplementedException();
        }

    }
}
