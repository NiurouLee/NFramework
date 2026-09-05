
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public class AbilityEffectAddStatusComponent : Entity
    {
        public CombatEntity Owner => GetParent<AbilityEffect>().Owner;

        public void OnAssignEffect(EffectAssignAction effectAssignAction)
        {
            if (this.Owner.AddStatusActionAbility.TryMakeAction(out var action))
            {
                effectAssignAction.FillDatasToAction(action);
                action.SourceAssignAction = effectAssignAction;
                action.ApplyStatus();
            }
        }
    }
}
