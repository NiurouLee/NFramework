using System;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 界面上的组件
    /// </summary>
    public interface IUIComponent
    {
        public RectTransform RectTransform => gameObject.GetComponent<RectTransform>();
        public GameObject gameObject { get; }
        public void UIComponentAwake();
        public void UIComponentDestroy();
    }

    public interface IUIInputComponent : IUIComponent
    {
    }

    public interface IUIInputTrigger<T>
    {
    }

    public interface IUIClickInputTrigger<T> : IUIInputTrigger<T>
    {
        public event Action<T> OnClickEvent;
        public void OnTriggerClick(T inComponent);
    }

    public interface IUIDoubleClickInputTrigger<T> : IUIInputTrigger<T>
    {
        public event Action<T> OnDoubleClickEvent;
        public void OnTriggerDoubleClick(T inComponent);
    }

    public interface IUILongPressInputTrigger<T> : IUIInputTrigger<T>
    {
        public float LongPressTime { get; set; }
        public event Action<T, float> OnLongPressEvent;
        public void OnTriggerLongPress(T inComponent, float inLongPressTime = 0);
    }

    public interface IUISelectInputTrigger<T> : IUIInputTrigger<T>
    {
        public bool IsSelected { get; set; }
        public event Action<T, bool> OnSelectEvent;
        public void OnSelectTrigger(bool isSelected);
    }
}