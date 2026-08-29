using System;
using XHFramework.Core;
using   NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// View上的输入组件，负责管理View上的输入事件订阅关系
    /// </summary>
    public class ViewInputComponent : ViewComponent
    {
        private UIInputComponentRecords m_inputComponents;

        public UIInputComponentRecords InputComponents
        {
            get
            {
                if (m_inputComponents == null)
                {
                    m_inputComponents = GetSystem<ObjectPoolSystem>().Alloc<UIInputComponentRecords>();
                    m_inputComponents.Awake();
                    m_inputComponents.SetView(this.View);
                }
                return m_inputComponents;
            }
        }
        public void BindClick<T>(T inComponent, Action<T> inCallback) where T : IUIClickInputTrigger<T>, IUIInputComponent
        {
            InputComponents.TryAdd(inComponent);
            inComponent.OnClickEvent += inCallback;
        }

        public override void OnViewComponentDestroy()
        {
            if (m_inputComponents != null)
            {
                m_inputComponents.Destroy();
                GetSystem<ObjectPoolSystem>().Free(m_inputComponents);
                m_inputComponents = null;
            }
        }
    }

    public static class ViewInputComponentExtension
    {
        public static void BindClick<T>(this View inView, T inComponent, Action<T> inCallback) where T : IUIClickInputTrigger<T>, IUIInputComponent
        {
            var component = ViewUtils.CheckAndAdd<ViewInputComponent>(inView);
            component.BindClick<T>(inComponent, inCallback);
        }


    }
}