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
            OnDestroy();
            DestroyFacade();
            DestroyComponentContainer();
        }

        protected virtual void OnDestroy()
        {
        }
    }
}