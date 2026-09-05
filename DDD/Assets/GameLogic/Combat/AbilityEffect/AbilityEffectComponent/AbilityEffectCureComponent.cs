
using NFramework.Core.Live;
using NFramework.Module.EntityModule;
using NFramework.Utils;
using UnityEngine;

namespace NFramework.Module.Combat
{
    public class AbilityEffectCureComponent : Entity
    {
        public CureEffect CureEffect => (CureEffect)GetParent<AbilityEffect>().effect;
        public string CureValueFormula => CureEffect.CureValueFormula;
        public CombatEntity Owner => GetParent<AbilityEffect>().Owner;
        public int GetCureValue()
        {
            return Mathf.CeilToInt(ExpressionUtil.Evalue<float>(CureValueFormula, GetParent<AbilityEffect>().GetParamsDict()));
        }
        public void OnAssignEffect(EffectAssignAction effectAssignAction)
        {
            if (Owner.CureActionAbility.TryMakeAction(out var action))
            {
                effectAssignAction.FillDatasToAction(action);
                {
                    action.ApplyCure();
                }
            }
        }
    }


}