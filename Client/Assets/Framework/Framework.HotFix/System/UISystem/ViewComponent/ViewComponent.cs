using NFramework;
using NFramework.ModuleSystem;
using NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 挂载在View上的组件，负责处理View的某个方面的逻辑
    /// </summary>
    public abstract class ViewComponent : NUIObject, IFreeToPool, IAwakeSystem<View, string>, IDestroySystem
    {
        public View View { get; private set; }

        public string Name { get; private set; }

        public virtual void Awake(View inView, string inName)
        {
            this.View = inView;
            this.Name = inName;
            this.OnViewComponentAwake();
        }

        public virtual void OnViewComponentAwake()
        {
        }

        public void Show()
        {
            this.OnViewComponentShow();
        }

        public virtual void OnViewComponentShow()
        {
        }

        public void Hide()
        {
            this.OnViewComponentHide();
        }

        public virtual void OnViewComponentHide()
        {
        }

        public void Reset()
        {
            this.OnViewComponentReset();
        }

        public virtual void OnViewComponentReset()
        {
        }

        public virtual void Destroy()
        {
            this.OnViewComponentDestroy();
            this.View = null;
            this.Name = string.Empty;
        }

        public virtual void OnViewComponentDestroy()
        {
        }

        public void FreeToPool()
        {
        }
    }
}