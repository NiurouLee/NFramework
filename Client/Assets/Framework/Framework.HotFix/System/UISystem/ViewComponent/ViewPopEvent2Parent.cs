using System;
using System.Collections.Generic;
using  NFramework.Core;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 与父级通信组件
    /// </summary>
    public class ViewPopEvent2ParentComponent : ViewComponent
    {
        private Dictionary<Type, System.Delegate> m_ViewDelegates;

        private Dictionary<Type, System.Delegate> Delegates
        {
            get
            {
                if (m_ViewDelegates == null)
                {
                    m_ViewDelegates = DictionaryPool.Alloc<Type, Delegate>();
                }

                return m_ViewDelegates;
            }
        }

        public bool RegisterSubEvent<T>(View2ParentEvent<T> inHandle) where T : IView2ParentEvent
        {
            var eventType = typeof(T);
            if (Delegates.TryGetValue(eventType, out var @delegate))
            {
                return false;
            }
            else
            {
                Delegates.Add(eventType, inHandle);
                return true;
            }
        }

        public void PopEvent2Parent<T>(ref T inEvent) where T : IView2ParentEvent
        {
            var parent = this.View.Parent;
            while (parent != null && parent != this.View)
            {
                if (parent.TryGetComponent<ViewPopEvent2ParentComponent>(out var component))
                {
                    component._OnChildPopEvent(ref inEvent);
                    return;
                }

                parent = parent.Parent;
            }
        }

        private void _OnChildPopEvent<T>(ref T inEvent) where T : IView2ParentEvent
        {
            if (this.m_ViewDelegates == null)
            {
                this.PopEvent2Parent(ref inEvent);
            }
            else
            {
                var eventType = typeof(T);
                if (this.Delegates.TryGetValue(eventType, out var @delegate) &&
                    @delegate is View2ParentEvent<T> func)
                {
                    if (func.Invoke(ref inEvent))
                    {
                        this.PopEvent2Parent(ref inEvent);
                    }
                }
            }
        }

        public override void OnViewComponentDestroy()
        {
            if (m_ViewDelegates != null)
            {
                m_ViewDelegates.Clear();
                DictionaryPool.Free(m_ViewDelegates);
                m_ViewDelegates = null;
            }
        }
    }

    public static class ViewPopEvent2ParentComponentExtensions
    {
        public static bool RegisterSubEvent<T>(this View inView, View2ParentEvent<T> inHandle)
            where T : IView2ParentEvent
        {
            var component = ViewUtils.CheckAndAdd<ViewPopEvent2ParentComponent>(inView);
            return component.RegisterSubEvent(inHandle);
        }

        public static void PopEvent2Parent<T>(this View inView, ref T inEvent) where T : IView2ParentEvent
        {
            var component = ViewUtils.CheckAndAdd<ViewPopEvent2ParentComponent>(inView);
            component.PopEvent2Parent(ref inEvent);
        }
    }
}