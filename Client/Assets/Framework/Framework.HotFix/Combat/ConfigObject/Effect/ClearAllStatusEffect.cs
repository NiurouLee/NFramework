using System;
using NFramework.ModuleSystem;

namespace NFramework.ModuleSystem.Combat
{
    [Serializable, Effect("清除所有状态", 10)]
    public class ClearAllStatusEffect : Effect
    {
        public override string Label => "清除所有状态";
    }
}