using NFramework.Module.EntityModule;
using UnityEngine;

namespace NFramework.Module.Combat
{
    /// <summary>
    ///运动组件， 这里管理战斗实体的移动，跳跃，击飞等运动功能 
    /// </summary>
    public class MotionComponent : Entity
    {
        public Vector3 Position { get => GetParent<CombatEntity>().TransformComponent.Position; set => GetParent<CombatEntity>().TransformComponent.Position = value; }
        public Quaternion Rotation { get => GetParent<CombatEntity>().TransformComponent.Rotation; set => GetParent<CombatEntity>().TransformComponent.Rotation = value; }

        public bool CanMove { get; set; }
        public long IdleTimer { get; set; }
        public long MoveTimer { get; set; }
        public Vector3 MoveVector { get; set; }
        public Vector3 OriginPos { get; set; }

        public void RunAI()
        {
        }
    }
}