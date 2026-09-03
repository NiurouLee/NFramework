using System;
using System.Collections;
using System.Collections.Generic;

namespace NFramework.ModuleSystem
{
    public delegate bool View2ParentEvent<T>(ref T view2ParentEvent);

    public static class ViewUtils
    {
        public static bool Check<T>(View inView, out T outComponent) where T : ViewComponent
        {
            if (inView.Has(ViewStateFlag.Components) & inView.TryGetComponent<T>(out outComponent))
            {
                return true;
            }
            outComponent = null;
            return false;
        }

        public static T CheckAndAdd<T>(View inView) where T : ViewComponent, new()
        {
            if (inView.TryGetComponent<T>(out var component))
            {
                return component;
            }

            component = inView.AddComponent<T>();
            return component;
        }

        /// <summary>
        /// 获取或创建指定 Name 的组件，方便同类型多 Name 组件的按需挂载。
        /// </summary>
        public static T CheckAndAdd<T>(View inView, string inName) where T : ViewComponent, new()
        {
            if (inView.TryGetComponent<T>(inName, out var component))
            {
                return component;
            }

            component = inView.AddComponent<T>(inName);
            return component;
        }

        public static bool GetContainer<T>(View inView, out T outContainer)
        {
            if (inView is T container)
            {
                outContainer = container;
                return true;
            }

            var parent = inView.Parent;
            if (parent == null || parent == inView)
            {
                outContainer = default;
                return false;
            }

            while (parent != null)
            {
                if (parent is T containerP)
                {
                    outContainer = containerP;
                    return true;
                }

                parent = parent.Parent;
            }

            outContainer = default;
            return false;
        }
    }
}
