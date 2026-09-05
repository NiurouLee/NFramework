
using NFramework.Core.Live;
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public class AbilityEffectActionTriggerComponent : Entity, IAwakeSystem,IDestroySystem
    {
        public Effect Effect => GetParent<AbilityEffect>().effect;
        public ActionPointType ActionPointType => Effect.ActionPointType;
        public CombatEntity Owner => GetParent<AbilityEffect>().Owner;

        public void Awake()
        {
            Owner.ListenActionPoint(ActionPointType, OnActionPointTrigger);
        }

        public void Destroy()
        {
            Owner.UnListenActionPoint(ActionPointType, OnActionPointTrigger);
        }

        private void OnActionPointTrigger(Entity action)
        {
            GetParent<AbilityEffect>().TryAssignEffectToOwner();
        }

    }
}