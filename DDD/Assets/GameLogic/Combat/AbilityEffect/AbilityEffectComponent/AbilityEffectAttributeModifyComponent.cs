
using NFramework.Core.Live;
using NFramework.Module.EntityModule;
using NFramework.Utils;

namespace NFramework.Module.Combat
{
    public class AbilityEffectAttributeModifyComponent : Entity, IAwakeSystem, IDestroySystem
    {
        public AttributeModifyEffect AttributeModifyEffect => (AttributeModifyEffect)GetParent<AbilityEffect>().effect;
        public string NumericValueFormula => AttributeModifyEffect.NumericValueFormula;
        public AttributeType AttributeType => AttributeModifyEffect.AttributeType;
        public CombatEntity Owner => GetParent<AbilityEffect>().Owner;
        public float value;

        public void Awake()
        {
            value = ExpressionUtil.Evalue<float>(NumericValueFormula, GetParent<AbilityEffect>().GetParamsDict());
            if (AttributeModifyEffect.ModifyType == ModifyType.Add)
            {
                Owner.GetComponent<AttributeComponent>().GetNumeric(AttributeType).FinalAdd += value;
            }
            if (AttributeModifyEffect.ModifyType == ModifyType.PercentAdd)
            {
                Owner.GetComponent<AttributeComponent>().GetNumeric(AttributeType).FinalPctAdd += value;
            }
        }
        public void Destroy()
        {
            if (AttributeModifyEffect.ModifyType == ModifyType.Add)
            {
                Owner.GetComponent<AttributeComponent>().GetNumeric(AttributeType).FinalAdd -= value;
            }
            if (AttributeModifyEffect.ModifyType == ModifyType.PercentAdd)
            {
                Owner.GetComponent<AttributeComponent>().GetNumeric(AttributeType).FinalPctAdd -= value;
            }
        }
    }
}