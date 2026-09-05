using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    /// <summary>
    /// 效果分配能力
    /// </summary>
    public class EffectAssignActionAbility : Entity, IActionAbility
    {
        public new bool Enable { get; set; }
        public CombatEntity Owner => GetParent<CombatEntity>();
        public bool TryMakeAction(out EffectAssignAction action)
        {
            if (!Enable)
            {
                action = null;
            }
            else
            {
                action = Owner.AddChild<EffectAssignAction>();
                action.ActionAbility = this;
                action.Creator = Owner;
            }
            return Enable;
        }
    }

    public class EffectAssignAction : Entity, IActionExecution
    {
        //释放这个赋予效果行动的能力（skill能力，status能力，Item能力，Attack能力）
        public AbilityEffect AbilityEffect { get; set; }
        public Entity SourceAbility;
        public IActionExecution ActionExecution;
        public IAbilityExecution AbilityExecution;
        public AbilityItem AbilityItem;
        public Entity ActionAbility { get; set; }
        public EffectAssignAction SourceAssignAction { get; set; }
        public CombatEntity Creator { get; set; }
        public CombatEntity Target { get; set; }

        private void PreProcess()
        {

        }

        public void ApplyEffectAssign()
        {
            PreProcess();
            AbilityEffect.StartAssignEffect(this);
            PostProcess();
            FinishAction();
        }

        public void FillDatasToAction(IActionExecution action)
        {
            action.SourceAssignAction = this;
            action.Target = Target;
        }

        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.AssignEffect, this);
            if (!Target.IsDisposed)
            {
                Target.TriggerActionPoint(ActionPointType.ReceiveEffect, this);
            }
        }
        public void FinishAction()
        {
            Dispose();
        }

    }
}