using System.Threading;
using Cysharp.Threading.Tasks;

namespace NFramework.ModuleSystem
{
    public interface IUIFacadeProvider
    {
        public UIFacade Alloc<T>() where T : View;
        public UIFacade Alloc(string inViewID);
        public UniTaskCompletionSource<UIFacade> AllocAsync<T>(CancellationToken inCancellationToken = default)
            where T : View;

        public UniTaskCompletionSource<UIFacade> AllocAsync(string inViewID,
            CancellationToken inCancellationToken = default);
        public void Free(UIFacade inUIFacade);
        public void Destroy();
    }
}
