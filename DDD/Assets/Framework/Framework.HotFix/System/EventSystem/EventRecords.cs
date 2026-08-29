using  NFramework.Core;
using    NFramework.ModuleSystem;
using   NFramework.ModuleSystem;

namespace NFramework.ModuleSystem.Module.EventModule
{
    public class EventRecords : BaseRecordSet<BaseRegister>, IEventScheduler, IFreeToPool
    {
        private IEventScheduler EventSchedule { get; set; }
        public void SetSchedule(IEventScheduler inEventSchedule)
        {
            EventSchedule = inEventSchedule;
        }

        public BaseRegister Subscribe<T>(RefAction<T> callback) where T : IEvent
        {
            var register = this.EventSchedule.Subscribe<T>(callback);
            this.TryAdd(register);
            return register;
        }

        public BaseRegister Subscribe<T>(RefAction<T> callback, RefFunc<T> condition) where T : IEvent
        {
            var register = this.EventSchedule.Subscribe<T>(callback, condition);
            this.TryAdd(register);
            return register;
        }

        public BaseRegister Subscribe<T>(RefAction<T> callback, string channel) where T : IEvent
        {
            var register = this.EventSchedule.Subscribe<T>(callback, channel);
            this.TryAdd(register);
            return register;
        }

        public void UnSubscribe<T>(RefAction<T> callback) where T : IEvent
        {
            var register = ((NObject)NFROOT.I).GetSystem<ObjectPoolSystem>().Alloc<NormalRegister>();
            register.EventType = typeof(T);
            register.CallBack = callback;
            register.EventSchedule = this.EventSchedule;
            this.EventSchedule.UnSubscribe(register);
            this.TryRemove(register);
        }

        public void UnSubscribe<T>(RefAction<T> callback, RefFunc<T> condition) where T : IEvent
        {
            var register = ((NObject)NFROOT.I).GetSystem<ObjectPoolSystem>().Alloc<ConditionRegister>();
            register.EventType = typeof(T);
            register.CallBack = callback;
            register.Condition = condition;
            register.EventSchedule = this.EventSchedule;
            this.EventSchedule.UnSubscribe(register);
            this.TryRemove(register);
        }

        public void UnSubscribe<T>(RefAction<T> callback, string channel) where T : IEvent
        {
            var register = ((NObject)NFROOT.I).GetSystem<ObjectPoolSystem>().Alloc<ChannelRegister>();
            register.EventType = typeof(T);
            register.CallBack = callback;
            register.Channel = channel;
            register.EventSchedule = this.EventSchedule;
            this.EventSchedule.UnSubscribe(register);
            this.TryRemove(register);
        }

        public void UnSubscribe(BaseRegister inRegister)
        {
            this.EventSchedule.UnSubscribe(inRegister);
            this.TryRemove(inRegister);
        }

        public bool Check<T>(RefAction<T> callback) where T : IEvent
        {
            return this.EventSchedule.Check<T>(callback);
        }

        public bool Check<T>(RefAction<T> callback, RefFunc<T> condition) where T : IEvent
        {
            return this.EventSchedule.Check<T>(callback, condition);
        }

        public bool Check<T>(RefAction<T> callback, string channel) where T : IEvent
        {
            return this.EventSchedule.Check<T>(callback, channel);
        }

        protected override void OnDestroy()
        {
            foreach (var register in this.Records)
            {
                this.EventSchedule.UnSubscribe(register);
            }
            this.EventSchedule = null;
        }

        public void FreeToPool()
        {
        }
    }
}