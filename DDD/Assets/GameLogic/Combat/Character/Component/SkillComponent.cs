using System.Collections.Generic;
using NFramework.Core.Live;
using NFramework.Module.EntityModule;
using NFramework.Module.ResModule;
using UnityEngine;

namespace NFramework.Module.Combat
{
    /// <summary>
    /// skill管理component,管理按键->Ability的映射。
    /// </summary>
    public class SkillComponent : Entity
    {
        public CombatEntity Combat => GetParent<CombatEntity>();
        public Dictionary<int, Ability> skillDict = new Dictionary<int, Ability>();
        public Dictionary<KeyCode, int> skillInputDict = new Dictionary<KeyCode, int>();
        public Ability AttachSkill(int skillId)
        {
            AbilityConfigObject skillConfigObject = null;//= NFROOT.I.G<ResM>().Load<AbilityConfigObject>(string.Empty);
            if (skillConfigObject == null)
            {
                return null;
            }

            var skill = Combat.AttachAbility<Ability>(skillConfigObject);
            if (!skillDict.ContainsKey(skill.SkillConfigObject.Id))
            {
                skillDict.Add(skill.SkillConfigObject.Id, skill);
            }
            return skill;
        }

        public void BindSkillInput(KeyCode inKeyCode, int inSkillId)
        {
            if (skillInputDict.ContainsKey(inKeyCode))
            {
                skillInputDict[inKeyCode] = inSkillId;
            }
            else
            {
                skillInputDict.Add(inKeyCode, inSkillId);
            }
        }

        public void UnBindSkillInput(KeyCode inKeyCode)
        {
            if (skillInputDict.ContainsKey(inKeyCode))
            {
                skillInputDict.Remove(inKeyCode);
            }
        }

        public Ability GetSkill(int skillId)
        {
            if (skillDict.TryGetValue(skillId, out var skill))
            {
                return skill;
            }
            return null;
        }
    }
}