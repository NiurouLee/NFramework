using Sirenix.OdinInspector;
namespace NFramework.Module.Combat
{
    public class RemoveStatusEffect : Effect
    {
        public override string Label
        {
            get
            {
                if (this.statusConfigObject != null)
                {
                    return $"移除[{statusConfigObject.Name}] 状态效果";
                }
                return "移除状态效果";
            }

        }
        [ToggleGroup("Enabled")]
        [LabelText("状态配置")]
        public StatusConfigObject statusConfigObject;
    }
}