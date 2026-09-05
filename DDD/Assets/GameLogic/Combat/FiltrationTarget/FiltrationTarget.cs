
using System.Collections.Generic;
using UnityEngine;

namespace NFramework.Module.Combat
{
    public static class FiltrationTarget
    {
        public static CombatEntity GetTarget(TransformComponent transformComponent, float distance, TagType tagType = TagType.Enemy)
        {
            // List<CombatEntity> list = NFROOT.I.G<CombatM>().CombatContext.GetCombatListByTag(tagType);
            List<CombatEntity> list =  null;
            if (list.Count == 0)
            {
                return null;
            }
            float minPos = distance;
            CombatEntity target = null;
            foreach (var item in list)
            {
                float temp = Vector3.Distance(item.TransformComponent.Position, transformComponent.Position);
                if (temp < minPos)
                {
                    minPos = temp;
                    target = item;
                }
            }
            if (target == null) return null;
            return target;
        }

        public static List<CombatEntity> GetTargetList(TransformComponent transformComponent, float distance, TagType tagType = TagType.Enemy)
        {
            List<CombatEntity> targetList = new List<CombatEntity>();
            // List<CombatEntity> list = NFROOT.I.G<CombatM>().CombatContext.GetCombatListByTag(tagType);
            List<CombatEntity> list = null;
            if (list.Count == 0)
            {
                return null;
            }

            foreach (CombatEntity item in list)
            {
                if (IsIncludeTarget(transformComponent, item.TransformComponent))
                {
                    float temp = Vector3.Distance(item.TransformComponent.Position, transformComponent.Position);
                    if (temp < distance)
                    {
                        targetList.Add(item);
                    }
                }
            }
            return targetList;
        }

        public static bool IsIncludeTarget(TransformComponent self, TransformComponent target)
        {
            Vector3 normalDistance = (target.Position - self.Position).normalized;
            if (self.Rotation.y == 1)
            {
                //自己朝向左边，只能打x比我小的
                if (normalDistance.x < 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //自己朝向右边，所以只能打x比我大的
                if (normalDistance.x < 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}