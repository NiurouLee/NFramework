using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// View
    /// </summary>
    public partial class View : NUIObject
    {
        public RectTransform RectTransform { get; private set; }

        public void Awake()
        {
            OnAwake();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected virtual void OnAwake()
        {
        }

        public virtual void Show()
        {
            OnShow();
            Visible();
        }

        protected virtual void OnShow()
        {
        }

        public virtual void Visible()
        {
            this.Facade?.Visible();
            OnVisible();
        }

        protected virtual void OnVisible()
        {
        }

        public virtual void Hide()
        {
            NotVisible();
            OnHide();
        }

        protected virtual void OnHide()
        {
        }

        public virtual void NotVisible()
        {
            this.Facade?.NotVisible();
            OnNotVisible();
        }

        protected virtual void OnNotVisible()
        {
        }

        public virtual void Focus()
        {
            OnFocus();
        }

        protected virtual void OnFocus()
        {
        }

        public virtual void NotFocus()
        {
            OnNotFocus();
        }

        protected virtual void OnNotFocus()
        {
        }

        public virtual void Destroy()
        {
            if (this.Has(ViewStateFlag.Destroy))
            {
                return;
            }

            this.Learn(ViewStateFlag.Destroy);

            // 独立销毁子 View 时，先从父 View 的 SubViewRecords 摘除，避免 records 里堆积已销毁对象；
            // 父 View 正在销毁时 records 本来就会清空，不能边遍历边 Remove。
            var parent = this.Parent;
            if (parent != null && !parent.Has(ViewStateFlag.Destroy) &&
                parent.TryGetComponent<ViewSubViewComponent>(out var subViewComponent))
            {
                subViewComponent.ViewRecords.TryRemove(this);
            }

            this.DestroyParent();
            OnDestroy();
            if (Facade != null)
            {
                DestroyFacade();
            }

            DestroyComponentContainer();
        }

        protected virtual void OnDestroy()
        {
        }
    }
}
