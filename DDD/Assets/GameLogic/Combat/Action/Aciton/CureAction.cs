using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    /// <summary>
    /// 治疗能力
    /// </summary>
    public class CureActionAbility : Entity, IActionAbility
    {
        public bool Enable { get; set; }
        public CombatEntity Owner => GetParent<CombatEntity>();

        public bool TryMakeAction(out CureAction action)
        {
            if (!Enable)
            {
                action = null;
            }
            else
            {
                action = Owner.AddChild<CureAction>();
                action.ActionAbility = this;
                action.Creator = Owner;
            }
            return Enable;
        }
    }

    /// <summary>
    /// 治疗行为
    /// </summary>
    public class CureAction : Entity, IActionExecution
    {
        public int CureValue;
        public Entity ActionAbility { get; set; }
        public EffectAssignAction SourceAssignAction { get; set; }
        public CombatEntity Creator { get; set; }
        public CombatEntity Target { get; set; }
        public void FinishAction()
        {
            Dispose();
        }

        private void PreProcess()
        {
            if (SourceAssignAction != null && SourceAssignAction.AbilityEffect != null)
            {
                CureValue = SourceAssignAction.AbilityEffect.GetComponent<AbilityEffectCureComponent>().GetCureValue();
            }
        }

        public void ApplyCure()
        {
            PreProcess();
            if (!Target.CurrentHealth.IsFull())
            {
                Target.ReceiveCure(this);
            }

            PostProcess();
            FinishAction();
        }

        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PostGiveCure, this);
            Target.TriggerActionPoint(ActionPointType.PostReceiveCure, this);
        }
    }



}