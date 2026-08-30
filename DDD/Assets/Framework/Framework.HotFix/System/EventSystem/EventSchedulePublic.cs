using   NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    public partial class EventSchedule
    {
        public void Publish<T>(ref T e) where T : IEvent
        {
            this.Fire(ref e);
        }

        public int GetCount<T>() where T : IEvent
        {
            var type = typeof(T);
            return this.Count(type);
        }


        public BaseRegister Subscribe<T>(RefAction<T> callback) where T : IEvent
        {
            var type = typeof(T);
            var register = GetSystem<ObjectPoolSystem>().Alloc<NormalRegister>();
            register.EventType = type;
            register.CallBack = callback;
            register.EventSchedule = this;
            return this._Subscribe(register);
        }

        public BaseRegister Subscribe<T>(RefAction<T> callback, RefFunc<T> condition) where T : IEvent
        {
            var type = typeof(T);
            var register = GetSystem<ObjectPoolSystem>().Alloc<ConditionRegister>();
            register.EventType = type;
            register.CallBack = callback;
            register.EventSchedule = this;
            register.Condition = condition;
            return this._Subscribe(register);
        }

        public BaseRegister Subscribe<T>(RefAction<T> callback, string channel) where T : IEvent
        {
            var type = typeof(T);
            var subscribe = GetSystem<ObjectPoolSystem>().Alloc<ChannelRegister>();

            subscribe.EventType = type;
            subscribe.CallBack = callback;
            subscribe.Channel = channel;
            subscribe.EventSchedule = this;
            return this._Subscribe(subscribe);
        }

        public void UnSubscribe<T>(RefAction<T> callback) where T : IEvent
        {
            var type = typeof(T);
            var register = GetSystem<ObjectPoolSystem>().Alloc<NormalRegister>();
            register.EventType = type;
            register.CallBack = callback;
            register.EventSchedule = this;
            this._Unsubscribe(register);
        }

        public void UnSubscribe<T>(RefAction<T> callback, RefFunc<T> condition) where T : IEvent
        {
            var type = typeof(T);
            var register = GetSystem<ObjectPoolSystem>().Alloc<ConditionRegister>();
            register.EventType = type;
            register.CallBack = callback;
            register.Condition = condition;
            register.EventSchedule = this;
            this._Unsubscribe(register);
        }

        public void UnSubscribe<T>(RefAction<T> callback, string channel) where T : IEvent
        {
            var type = typeof(T);
            var subscribe = GetSystem<ObjectPoolSystem>().Alloc<ChannelRegister>();

            subscribe.EventType = type;
            subscribe.CallBack = callback;
            subscribe.Channel = channel;
            subscribe.EventSchedule = this;
            this._Unsubscribe(subscribe);
        }

        public void UnSubscribe(BaseRegister inRegister)
        {
            if (inRegister != null)
            {
                this._Unsubscribe(inRegister);
            }
        }


        public bool Check<T>(RefAction<T> callback) where T : IEvent
        {
            var type = typeof(T);
            var register = GetSystem<ObjectPoolSystem>().Alloc<NormalRegister>();
            register.EventType = type;
            register.CallBack = callback;
            register.EventSchedule = this;
            return this.Check(register);
        }

        public bool Check<T>(RefAction<T> callback, RefFunc<T> condition) where T : IEvent
        {
            var type = typeof(T);
            var register = GetSystem<ObjectPoolSystem>().Alloc<ConditionRegister>();
            register.EventType = type;
            register.CallBack = callback;
            register.Condition = condition;
            register.EventSchedule = this;
            return this.Check(register);
        }

        public bool Check<T>(RefAction<T> callback, string channel) where T : IEvent
        {
            var type = typeof(T);
            var subscribe = GetSystem<ObjectPoolSystem>().Alloc<ChannelRegister>();
            subscribe.EventType = type;
            subscribe.CallBack = callback;
            subscribe.Channel = channel;
            subscribe.EventSchedule = this;
            return this.Check(subscribe);
        }
    }
}