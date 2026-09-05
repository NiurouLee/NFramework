
using System.Collections.Generic;
namespace NFramework.ModuleSystem.Combat
{
    public class AbilityEffectConditionTriggerComponent : Entity, IAwakeSystem, IDestroySystem
    {
        public Effect Effect => GetParent<AbilityEffect>().effect;
        public string ConditionValueFromula => ParseParams(Effect.ConditioNValueFormula, GetParent<AbilityEffect>().GetParamsDict());
        public ConditionType ConditionType => Effect.ConditionType;
        public CombatEntity Owner => GetParent<AbilityEffect>().Owner;

        public void Awake()
        {
            Owner.ListenCondition(ConditionType, OnConditionTrigger, ConditionValueFromula);
        }

        public void Destroy()
        {
            Owner.UnListenCondition(ConditionType, OnConditionTrigger);
        }
        private void OnConditionTrigger()
        {
            GetParent<AbilityEffect>().TryAssignEffectToOwner();
        }

        private string ParseParams(string origin, Dictionary<string, string> paramsDict)
        {
            string temp = origin;
            foreach (var item in paramsDict)
            {
                if (!string.IsNullOrEmpty(temp))
                {
                    temp = temp.Replace(item.Key, item.Value);
                }
            }
            return temp;
        }
    }
}