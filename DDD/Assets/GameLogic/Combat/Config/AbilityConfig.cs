
using NFramework.Module.Config.DataPipeline;

namespace Logic
{
    public partial class AbilityConfig : IConfig
    {
        public int Id { get; set; }
        public string KeyName;
        public string Type;
        public string TargetGroup;
        public string TargetSelect;
        public float CoolDown;
        public string Description;
        public string BuffType;
        public string StatusSlot;
        public string CanStack;

    }
}