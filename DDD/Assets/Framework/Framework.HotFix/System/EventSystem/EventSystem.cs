
namespace NFramework.ModuleSystem
{
    public class EventSystem : FrameworkSystemModuleBase ,IEventScheduler
    {
        public EventSchedule D = new EventSchedule();

        public override void Awake()
        {
            D = new EventSchedule();
        }

        public BaseRegister Subscribe<T>(RefAction<T> callback) where T : IEvent
        {
            throw new System.NotImplementedException();
        }

        public BaseRegister Subscribe<T>(RefAction<T> callback, RefFunc<T> condition) where T : IEvent
        {
            throw new System.NotImplementedException();
        }

        public BaseRegister Subscribe<T>(RefAction<T> callback, string channel) where T : IEvent
        {
            throw new System.NotImplementedException();
        }

        public void UnSubscribe<T>(RefAction<T> callback) where T : IEvent
        {
            throw new System.NotImplementedException();
        }

        public void UnSubscribe<T>(RefAction<T> callback, RefFunc<T> condition) where T : IEvent
        {
            throw new System.NotImplementedException();
        }

        public void UnSubscribe<T>(RefAction<T> callback, string channel) where T : IEvent
        {
            throw new System.NotImplementedException();
        }

        public void UnSubscribe(BaseRegister inRegister)
        {
            throw new System.NotImplementedException();
        }

        public bool Check<T>(RefAction<T> callback) where T : IEvent
        {
            throw new System.NotImplementedException();
        }

        public bool Check<T>(RefAction<T> callback, RefFunc<T> condition) where T : IEvent
        {
            throw new System.NotImplementedException();
        }

        public bool Check<T>(RefAction<T> callback, string channel) where T : IEvent
        {
            throw new System.NotImplementedException();
        }

        public void Fire<T>(ref T @event) where T : IEvent
        {
            throw new System.NotImplementedException();
        }
    }
}