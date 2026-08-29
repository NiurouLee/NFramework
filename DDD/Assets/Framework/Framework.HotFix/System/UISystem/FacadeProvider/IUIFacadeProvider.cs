using Cysharp.Threading.Tasks;

namespace NFramework.ModuleSystem
{
    public interface IUIFacadeProvider
    {
        public UIFacade Alloc<T>() where T : View;
        public UIFacade Alloc(string inViewID);
        public UniTaskCompletionSource<UIFacade> AllocAsync<T>() where T : View;
        public UniTaskCompletionSource<UIFacade> AllocAsync(string inViewID);
        public void Free(UIFacade inUIFacade);
        public void Destroy();
    }
}