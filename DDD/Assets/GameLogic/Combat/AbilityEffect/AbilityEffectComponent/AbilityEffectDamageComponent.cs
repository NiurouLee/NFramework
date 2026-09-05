using NFramework.Core.Live;
using NFramework.Module.EntityModule;
using UnityEngine;
using NFramework.Utils;

namespace NFramework.Module.Combat
{
    public class AbilityEffectDamageComponent : Entity
    {
        public DamageEffect DamageEffect => GetParent<AbilityEffect>().effect as DamageEffect;
        public string DamageValueFormula => DamageEffect.DamageValueFormula;
        public CombatEntity Owner => GetParent<AbilityEffect>().Owner;
        public int GetDamageValue()
        {

            return Mathf.CeilToInt(ExpressionUtil.Evalue<float>(DamageValueFormula, GetParent<AbilityEffect>().GetParamsDict()));
        }

        public void OnAssignEffect(EffectAssignAction effectAssigAction)
        {
            if (Owner.DamageActionAbility.TryMakeAction(out var damageAction))
            {
                effectAssigAction.FillDatasToAction(damageAction);
                damageAction.DamageSource = DamageSource.Skill;
                damageAction.ApplyDamage();
            }
        }
    }
}