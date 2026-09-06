using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    public class CustomSlider : Slider, IBeginDragHandler, IEndDragHandler
    {
        public event Action<PointerEventData> OnBeginDragEvent;

        public event Action<PointerEventData> OnEndDragEvent;

        public event Action<PointerEventData> OnPointerDownEvent;


        public override float value
        {
            get => base.value;
            set
            {
                base.value = Mathf.Clamp(value, 0, maxValue);
                if (value == 0)
                {
                    try
                    {
                        fillRect.gameObject.SetActive(false);
                    }
                    catch (Exception e)
                    {
                    }
                }
                else
                {
                    try
                    {
                        fillRect.gameObject.SetActive(true);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            this.OnBeginDragEvent?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            this.OnEndDragEvent?.Invoke(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            this.OnPointerDownEvent?.Invoke(eventData);
        }
    }
}