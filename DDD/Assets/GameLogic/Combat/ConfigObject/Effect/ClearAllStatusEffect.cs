using System;
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    [Serializable, Effect("清除所有状态", 10)]
    public class ClearAllStatusEffect : Effect
    {
        public override string Label => "清除所有状态";
    }
}