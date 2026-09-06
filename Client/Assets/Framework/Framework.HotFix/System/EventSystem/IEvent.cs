namespace NFramework.ModuleSystem
{
    public interface IEvent
    {
    }

    public interface IChannelEvent : IEvent
    {
        public string Channel { get; }
    }
}