using System;
using   NFramework.ModuleSystem;
using    NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    public abstract class BaseRegister : NObject, IFreeToPool, IEquatable<BaseRegister>
    {
        public System.Type EventType { get; set; }
        public System.Delegate CallBack { get; set; }
        public IEventScheduler EventSchedule { get; set; }
        public abstract void UnRegister();

        internal void Invoke<T>(T e) where T : IEvent
        {
            var type = typeof(T);
            if (CallBack is RefAction<T> action)
            {
                action(ref e);
            }
        }

        public virtual void FreeToPool()
        {
            this.EventSchedule = null;
            this.CallBack = null;
            this.EventType = null;
        }
        public bool Equals(BaseRegister other)
        {
            if (other == null)
                return false;
            return this.EventType == other.EventType && this.CallBack == other.CallBack && this.EventSchedule == other.EventSchedule;
        }
    }

    public class NormalRegister : BaseRegister, IEquatable<NormalRegister>
    {
        public bool Equals(NormalRegister other)
        {
            return base.Equals(other);
        }

        public override void UnRegister()
        {
            var inRegister = this.GetSystem<ObjectPoolSystem>().Alloc<NormalRegister>();
            inRegister.EventType = this.EventType;
            inRegister.CallBack = this.CallBack;
            inRegister.EventSchedule = this.EventSchedule;
            this.EventSchedule.UnSubscribe(inRegister);
        }

        public override void FreeToPool()
        {
            base.FreeToPool();
            this.GetSystem<ObjectPoolSystem>().Free(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is NormalRegister normalRegister)
                return this.Equals(normalRegister);
            return false;
        }

    }

    public class ConditionRegister : BaseRegister, IEquatable<ConditionRegister>
    {
        public System.Delegate Condition { get; set; }

        public bool Equals(ConditionRegister other)
        {
            if (other == null)
                return false;
            return this.Condition == other.Condition && base.Equals(other);
        }

        public override void UnRegister()
        {
            var inRegister = this.GetSystem<ObjectPoolSystem>().Alloc<ConditionRegister>();
            inRegister.EventType = this.EventType;
            inRegister.CallBack = this.CallBack;
            inRegister.EventSchedule = this.EventSchedule;
            inRegister.Condition = Condition;
            this.EventSchedule.UnSubscribe(inRegister);
        }

        public override void FreeToPool()
        {
            this.Condition = default;
            base.FreeToPool();
            this.GetSystem<ObjectPoolSystem>().Free(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is ConditionRegister conditionRegister)
                return this.Equals(conditionRegister);
            return false;
        }
    }

    public class ChannelRegister : BaseRegister, IEquatable<ChannelRegister>
    {
        public string Channel { get; set; }
        public bool Equals(ChannelRegister other)
        {
            if (other == null)
                return false;
            return base.Equals(other) && this.Channel == other.Channel;
        }

        public override void UnRegister()
        {
            var inRegister = this.GetSystem<ObjectPoolSystem>().Alloc<ChannelRegister>();
            inRegister.EventType = this.EventType;
            inRegister.CallBack = this.CallBack;
            inRegister.EventSchedule = this.EventSchedule;
            inRegister.Channel = this.Channel;
            this.EventSchedule.UnSubscribe(inRegister);
        }

        public override void FreeToPool()
        {
            this.Channel = default;
            base.FreeToPool();
            this.GetSystem<ObjectPoolSystem>().Free(this);

        }
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is ChannelRegister channelRegister)
                return this.Equals(channelRegister);
            return false;
        }
    }
}