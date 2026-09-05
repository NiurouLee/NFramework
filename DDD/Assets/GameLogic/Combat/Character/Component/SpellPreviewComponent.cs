using NFramework.Core.Live;
using NFramework.Module.Combat;
using NFramework.Module.EntityModule;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEngine;

namespace NFramework.Module.Combat
{
    /// <summary>
    /// 技能示范预览组件 
    /// </summary>
    public class SpellPreviewComponent : Entity, IUpdateSystem
    {
        public CombatEntity OwnerEntity => GetParent<CombatEntity>();
        public SkillComponent SkillComponent => OwnerEntity.GetComponent<SkillComponent>();
        public SpellSkillComponent SpellSkillComponent => OwnerEntity.GetComponent<SpellSkillComponent>();
        private bool Previewing { get; set; }
        private Ability PreviewingSkill { get; set; }

        public void Update(float deltaTime)
        {
            var skillComponent = OwnerEntity.GetComponent<SkillComponent>();
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Cursor.visible = false;
                var skillId = skillComponent.skillInputDict[KeyCode.Q];
                var skill = skillComponent.GetSkill(skillId);
                PreviewingSkill = skill;
                EnterPreview();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Cursor.visible = false;
                var skillId = skillComponent.skillInputDict[KeyCode.W];
                var skill = skillComponent.GetSkill(skillId);
                PreviewingSkill = skill;
                EnterPreview();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Cursor.visible = false;
                var skillId = skillComponent.skillInputDict[KeyCode.E];
                var skill = skillComponent.GetSkill(skillId);
                PreviewingSkill = skill;
                EnterPreview();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Cursor.visible = false;
                var skillId = skillComponent.skillInputDict[KeyCode.R];
                var skill = skillComponent.GetSkill(skillId);
                PreviewingSkill = skill;
                EnterPreview();
            }
        }
        public void EnterPreview()
        {
            CancelPreview();
            Previewing = true;
            var targetSelectType = SkillTargetSelectType.Custom;
            var AffectTargetType = SkillAffectTargetType.EnemyTeam;
            var skillId = PreviewingSkill.SkillConfigObject.Id;
            OnSelectSelf();
        }

        public void OnSelectSelf()
        {
            CombatEntity combatEntity = this.GetParent<CombatEntity>();
            this.SpellSkillComponent.SpellWithTarget(PreviewingSkill, combatEntity);
        }

        public void OnSelectTarget(GameObject selectGameObject)
        {
            CancelPreview();
            NFROOT.I.GetModule<CombatM>().CurrentContext.GameObject2CombatEntity.TryGetValue(selectGameObject, out CombatEntity combatEntity);
            SpellSkillComponent.SpellWithTarget(PreviewingSkill, combatEntity);
        }

        public void OnInputDirection(float direction, Vector3 point)
        {
            // OnInputPoint(point);
        }

        public void SelectTargetsWithDistance(Ability spellSkill, float distance)
        {
            var pos = this.OwnerEntity.TransformComponent.Position;
            if (OwnerEntity.SpellSkillActionAbility.TryMakeAction(out var action))
            {
                var enemiesRoot = GameObject.Find("Enemies");
                foreach (Transform item in enemiesRoot.transform)
                {
                    if (Vector3.Distance(item.position, pos) < distance)
                    {
                        action.Target = item.GetComponent<CombatEntity>();
                    }
                }
                if (action.Target != null)
                {
                    action.FinishAction();
                }

                action.SkillAbility = spellSkill;
                action.SpellSkill(false);
            }
        }


        public void CancelPreview()
        {
            Previewing = false;

        }







    }
}