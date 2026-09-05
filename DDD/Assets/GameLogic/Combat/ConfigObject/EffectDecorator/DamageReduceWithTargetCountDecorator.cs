using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NFramework.Module.Combat
{
    [Serializable, EffectDecorate("按命中目标数递减增加百分比伤害", 10)]
    public class DamageReduceWithTargetCountDecorator : EffectDecorator
    {
        [HideInInspector]
        public override string Label => "伤害减免";

        [ToggleGroup("Enabled"), LabelText("递减百分比")]
        public float ReducePercent;

        [ToggleGroup("Enable"), LabelText("伤害下限百分比")]
        public float MinPercent;
    }
}
