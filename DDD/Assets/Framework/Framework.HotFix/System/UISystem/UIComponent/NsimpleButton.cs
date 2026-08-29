using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NFramework.ModuleSystem
{
    public class NSimpleButton : Button, IUIInputComponent, IUIClickInputTrigger<NSimpleButton>
    {
        public event Action<NSimpleButton> OnClickEvent;

        public void UIComponentAwake()
        {
            this.onClick.AddListener(this.Click);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!IsInteractable()) return;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!IsInteractable()) return;
        }


        private void Click()
        {
            this.OnClickEvent?.Invoke(this);
        }


        public void OnTriggerClick(NSimpleButton inComponent)
        {
            this.onClick?.Invoke();
        }

        public void UIComponentDestroy()
        {
            this.onClick.RemoveListener(this.Click);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.onClick.RemoveListener(this.Click);
        }
    }
}