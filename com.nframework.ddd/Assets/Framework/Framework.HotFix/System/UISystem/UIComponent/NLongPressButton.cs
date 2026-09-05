using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NFramework.ModuleSystem
{
    public class NLongPressButton : Button, IUIComponent, IUIClickInputTrigger<NLongPressButton>, IUIDoubleClickInputTrigger<NLongPressButton>, IUILongPressInputTrigger<NLongPressButton>
    {

        public float singleClickIntervalTime = 0.3f;
        public float doubleClickIntervalTime = 0.3f;
        public float longClickTime = 1;
        public float longPressIntervalTime = 0.3f;
        private bool isPointDown = false;
        private float lastInvokeTime;
        private float lastUpTime;
        private float lastDownTime;
        private float downTime;
        private float upTime;
        public float LongPressTime { get; set; } = 1;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (!IsInteractable()) return;
            lastDownTime = downTime;
            downTime = Time.time;
            isPointDown = true;
            lastInvokeTime = downTime;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            isPointDown = false;
        }

        public void SetGray(bool interactable = true)
        {
            this.interactable = interactable;
        }

        public void SetNormal()
        {
            interactable = true;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (!IsInteractable()) return;
            var time = Time.time;
            lastUpTime = upTime;
            upTime = time;
            CheckSingleClick();
            CheckDoubleClick();
            CheckLongClick();
            isPointDown = false;
        }

        private bool CheckLongClick()
        {
            var pressTime = upTime - downTime;
            if (pressTime < longClickTime) return false;
            OnLongPressEvent?.Invoke(this, pressTime);
            return true;
        }

        private bool CheckSingleClick()
        {
            if (upTime - lastUpTime < singleClickIntervalTime) return false;
            OnClickEvent?.Invoke(this);
            return true;
        }

        private int clickCount;

        private bool CheckDoubleClick()
        {
            if (upTime - lastUpTime > doubleClickIntervalTime)
            {
                clickCount = 1;
                return false;
            }

            clickCount++;
            if (clickCount >= 2)
            {
                OnDoubleClickEvent?.Invoke(this);
                clickCount = 0;
                return true;
            }

            return false;
        }

        void Update()
        {
            if (!isPointDown) return;
            if (!(Time.time - downTime >= longClickTime)) return;
            if (Time.time - lastInvokeTime > longPressIntervalTime)
            {
                OnLongPressEvent?.Invoke(this, Time.time - downTime);
                lastInvokeTime = Time.time;
            }
        }

        public event Action<NLongPressButton> OnClickEvent;
        public event Action<NLongPressButton> OnDoubleClickEvent;
        public event Action<NLongPressButton, float> OnLongPressEvent;

        public void UIComponentAwake()
        {
        }

        public void UIComponentDestroy()
        {
        }

        public void OnTriggerClick(NLongPressButton inComponent)
        {

        }

        public void OnTriggerDoubleClick(NLongPressButton inComponent)
        {
        }

        public void OnTriggerLongPress(NLongPressButton inComponent)
        {
        }

        public void OnTriggerLongPress(NLongPressButton inComponent, float inLongPressTime = 0)
        {
            throw new NotImplementedException();
        }
    }
}


