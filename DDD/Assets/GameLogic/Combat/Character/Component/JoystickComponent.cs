using NFramework.Core.Live;
using NFramework.Module.EntityModule;
using UnityEngine;

namespace NFramework.Module.Combat
{
    public class JoystickComponent : Entity, IFixedUpdateSystem
    {

        public CombatEntity Combat => GetParent<CombatEntity>();
        public TransformComponent TransformComponent => Combat.TransformComponent;
        public AnimationComponent AnimationComponent => Combat.AnimationComponent;
        public float speed => Combat.AttributeComponent.MoveSpeed.Value;
        public Vector2 normalDistance;
        public void FixedUpdate(float deltaTime)
        {
            if (normalDistance != Vector2.zero)
            {
                TransformComponent.TransLate(normalDistance * deltaTime * speed);
            }
        }

        public void Move(Vector2 normalDistance, float angle)
        {
            this.normalDistance = normalDistance;
            if (normalDistance != Vector2.zero)
            {
                AnimationComponent.PlayAnimation(AnimationType.Walk);
            }
            else
            {
                AnimationComponent.PlayAnimation(AnimationType.Idle);
            }
            if (angle == 0) return;
            if (angle < 0)
            {
                TransformComponent.Rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                TransformComponent.Rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}