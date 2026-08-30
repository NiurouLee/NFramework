using NFramework.ModuleSystem;
using   NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// View上的事件记录组件，负责管理View上的事件订阅关系
    /// </summary>
    public class EventRecordsComponent : ViewComponent
    {
        private EventRecords m_viewEventRecords;

        public EventRecords Event
        {
            get
            {
                if (m_viewEventRecords == null)
                {
                    m_viewEventRecords = GetSystem<ObjectPoolSystem>().Alloc<EventRecords>();
                    m_viewEventRecords.Awake();
                    m_viewEventRecords.SetSchedule(GetSystem<EventSystem>().D);
                }

                return m_viewEventRecords;
            }
        }

        public override void OnViewComponentDestroy()
        {
            if (m_viewEventRecords != null)
            {
                m_viewEventRecords.Destroy();
                GetSystem<ObjectPoolSystem>().Free(m_viewEventRecords);
                m_viewEventRecords = null;
            }
        }
    }

    public static class ViewEventRecordsComponentExtensions
    {
        public static void Subscribe<T>(this View inView, RefAction<T> callback) where T : IEvent
        {
            var component = ViewUtils.CheckAndAdd<EventRecordsComponent>(inView);
            component.Event.Subscribe<T>(callback);
        }

        public static void Subscribe<T>(this View inView, RefAction<T> callback, RefFunc<T> condition) where T : IEvent
        {
            var component = ViewUtils.CheckAndAdd<EventRecordsComponent>(inView);
            component.Event.Subscribe<T>(callback, condition);
        }

        public static void Subscribe<T>(this View inView, RefAction<T> callback, string channel) where T : IEvent
        {
            var component = ViewUtils.CheckAndAdd<EventRecordsComponent>(inView);
            component.Event.Subscribe<T>(callback, channel);
        }

        public static void UnSubscribe<T>(this View inView, RefAction<T> callback) where T : IEvent
        {
            var component = ViewUtils.CheckAndAdd<EventRecordsComponent>(inView);
            component.Event.UnSubscribe<T>(callback);
        }

        public static void UnSubscribe<T>(this View inView, RefAction<T> callback, RefFunc<T> condition)
            where T : IEvent
        {
            var component = ViewUtils.CheckAndAdd<EventRecordsComponent>(inView);
            component.Event.UnSubscribe<T>(callback, condition);
        }

        public static void UnSubscribe<T>(this View inView, RefAction<T> callback, string channel) where T : IEvent
        {
            var component = ViewUtils.CheckAndAdd<EventRecordsComponent>(inView);
            component.Event.UnSubscribe<T>(callback, channel);
        }
    }
}
