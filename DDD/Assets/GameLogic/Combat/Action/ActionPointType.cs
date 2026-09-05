using System;
using Sirenix.OdinInspector;

namespace NFramework.Module.Combat
{
    [Flags]
    public enum ActionPointType
    {
        [LabelText("无")]
        None = 0,

        [LabelText("造成伤害前")]
        PreCauseDamage = 1 << 1,
        [LabelText("承受伤害前")]
        PreReceiveDamage = 1 << 2,

        [LabelText("造成伤害后")]
        PostCauseDamage = 1 << 3,

        [LabelText("承受伤害后")]
        PostReceiveDamage = 1 << 4,

        [LabelText("给与治疗前")]
        PostGiveCure = 1 << 5,

        [LabelText("接受治疗后")]
        PostReceiveCure = 1 << 6,

        [LabelText("赋予技能效果")]
        AssignEffect = 1 << 7,

        [LabelText("接受技能效果")]
        ReceiveEffect = 1 << 8,

        [LabelText("赋予状态后")]
        PostGiveStatus = 1 << 9,

        [LabelText("接受状态后")]
        PostReceiveStatus = 1 << 10,

        [LabelText("给予普攻前")]
        PreGiveAttack = 1 << 11,

        [LabelText("给予普攻后")]
        PostGiveAttack = 1 << 12,

        [LabelText("接受普攻前")]
        PreReceiveAttack = 1 << 13,

        [LabelText("接受普攻后")]
        PostReceiveAttack = 1 << 14,

        [LabelText("起跳前")]
        PreJumpTo = 1 << 15,

        [LabelText("起跳后")]
        PostJumpTo = 1 << 16,

        [LabelText("施法前")]
        PreSpell = 1 << 17,

        [LabelText("施法后")]
        PostSpell = 1 << 18,

        [LabelText("赋给普攻效果前")]
        preGiveAttackEffect = 1 << 19,

        [LabelText("赋给普攻效果后")]
        PostGiveAttackEffect = 1 << 20,

        [LabelText("承受普攻前")]
        PreReceiveAttackEffect = 1 << 21,

        [LabelText("承受普攻后")]
        PostReceiveAttackEffect = 1 << 22,

        [LabelText("赋给物品前")]
        PreGiveItem = 1 << 23,

        [LabelText("赋给物品后")]
        PostGiveItem = 1 << 24,

        [LabelText("承受物品前")]
        PreReceiveItem = 1 << 25,

        [LabelText("承受物品后")]
        PostReceiveItem = 1 << 26,

        Max,


    }
}