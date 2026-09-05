using System.Collections.Generic;
using UnityEngine;

namespace NFramework.ModuleSystem.Combat
{
    public class CombatContext : Entity, IAwakeSystem
    {
        public Dictionary<long, CombatEntity> combatDic = new();
        public Dictionary<long, AbilityItem> abilityItemDict = new();
        public Dictionary<GameObject, CombatEntity> GameObject2CombatEntity = new();

        public void Awake()
        {

        }
        public CombatEntity AddCombat(long inID, CombatTagType inTagType)
        {
            CombatEntity combat = AddChild<CombatEntity>();
            combat.AddComponent<CombatTagComponent, CombatTagType>(inTagType);
            combatDic.Add(inID, combat);
            return combat;
        }

        public CombatEntity GetCombat(long inID)
        {
            combatDic.TryGetValue(inID, out CombatEntity combat);
            return combat;
        }

        public void RemoveCombat(long inID)
        {
            if (combatDic.TryGetValue(inID, out CombatEntity combat))
            {
                this.RemoveChild(combat.Id);
                combatDic.Remove(inID);
            }
        }

        public void GetCombatByTag(CombatTagType inTagType, ref List<CombatEntity> outCombatList)
        {
            foreach (var combat in combatDic)
            {
                if (combat.Value.GetComponent<CombatTagComponent>().tagType == inTagType)
                {
                    outCombatList.Add(combat.Value);
                }
            }
        }

        public AbilityItem AddAbilityItem(SkillExecution skillExecution, ExecuteClipData data)
        {
            AbilityItem abilityItem = AddChild<AbilityItem, SkillExecution, ExecuteClipData>(skillExecution, data);
            if (!abilityItemDict.ContainsKey(abilityItem.Id))
            {
                abilityItemDict.Add(abilityItem.Id, abilityItem);
            }
            return abilityItem;
        }

        public void RemoveAbilityItem(long id)
        {
            var abilityItem = GetAbilityItem(id);
            if (abilityItem != null)
            {
                abilityItem.Dispose();
                abilityItemDict.Remove(id);
            }
        }

        public AbilityItem GetAbilityItem(long id)
        {
            abilityItemDict.TryGetValue(id, out AbilityItem abilityItem);
            return abilityItem;
        }

        public List<CombatEntity> GetCombatListByTag(TagType tagType)
        {
            List<CombatEntity> list = new List<CombatEntity>();
            foreach (var combat in combatDic.Values)
            {
                if (combat.TagComponent.tagType == tagType)
                {
                    list.Add(combat);
                }
            }
            return list;
        }


        #region  Debug
        public Dictionary<GameObject, CombatEntity> GameObject2Entity { get; set; } = new Dictionary<GameObject, CombatEntity>();
        public Dictionary<GameObject, AbilityItem> GameObject2AbilityItems { get; set; } = new Dictionary<GameObject, AbilityItem>();

        #endregion
    }

}