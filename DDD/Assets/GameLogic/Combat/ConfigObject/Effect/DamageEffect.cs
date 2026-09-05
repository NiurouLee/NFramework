using Sirenix.OdinInspector;

namespace NFramework.Module.Combat
{
    [LabelText("伤害类型")]
    public enum DamageType
    {
        [LabelText("物理伤害")]
        Physic = 0,
        [LabelText("魔法伤害")]
        Magic = 1,
        Real = 2,
    }

    [Effect("造成伤害", 10)]
    public class DamageEffect : Effect
    {
        public override string Label => "造成伤害";

        [ToggleGroup("Enable")]
        public DamageType DamageType;

        [ToggleGroup("Enable"), LabelText("取值")]
        public string DamageValueFormula;


        [ToggleGroup("Enabled"), LabelText("能否暴击")]
        public bool CanCrit;
    }
}
