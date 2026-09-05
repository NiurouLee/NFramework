using System;
using System.Collections.Generic;

namespace NFramework.ModuleSystem.Combat
{
    public class ConditionComponent : Entity
    {
        private Dictionary<long, Condition> _conditionDice = new Dictionary<long, Condition>();

        public void AddListener(ConditionType type, Action action, object obj = null)
        {
        }

        public void RemoveListener(ConditionType type, Action action)
        {
        }
    }
}