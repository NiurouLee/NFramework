namespace NFramework.ModuleSystem
{
    public interface IEventScheduler
    {
        public BaseRegister Subscribe<T>(RefAction<T> callback) where T : IEvent;

        public BaseRegister Subscribe<T>(RefAction<T> callback, RefFunc<T> condition) where T : IEvent;

        public BaseRegister Subscribe<T>(RefAction<T> callback, string channel) where T : IEvent;

        public void UnSubscribe<T>(RefAction<T> callback) where T : IEvent;

        public void UnSubscribe<T>(RefAction<T> callback, RefFunc<T> condition) where T : IEvent;

        public void UnSubscribe<T>(RefAction<T> callback, string channel) where T : IEvent;

        public void UnSubscribe(BaseRegister inRegister);

        public bool Check<T>(RefAction<T> callback) where T : IEvent;

        public bool Check<T>(RefAction<T> callback, RefFunc<T> condition) where T : IEvent;

        public bool Check<T>(RefAction<T> callback, string channel) where T : IEvent;

        public void Fire<T>(ref T @event) where T : IEvent;
    }
}