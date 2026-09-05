using Sirenix.OdinInspector;

namespace NFramework.Module.Combat
{
    public enum ActionControlType
    {
        [LabelText("(空)")]
        None = 0,

        [LabelText("移动禁止")]
        MoveForbid = 1 << 1,

        [LabelText("施法禁止")]
        SkillForbid = 1 << 2,

        [LabelText("攻击禁止")]
        AttackForbid = 1 << 3,

        [LabelText("移动控制")]
        MoveControl = 1 << 4,

        [LabelText("攻击控制")]
        AttackControl = 1 << 5,
    }


    public class ActionControlEffect : Effect
    {
        public override string Label => "行为控制";
        [ToggleGroup("Enable")]
        public ActionControlType ActionControlType;
    }
}