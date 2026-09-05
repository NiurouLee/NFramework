using   NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    public partial class UISystem
    {
        public T CreateViewComponent<T>() where T : ViewComponent, new()
        {
            var component = GetSystem<ObjectPoolSystem>().Alloc<T>();
            return component;
        }
    }
}