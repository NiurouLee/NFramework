using NFramework.Core.Live;
using NFramework.Module.EntityModule;
using NFramework.Module.TimerModule;
using UnityEngine;
using DG.Tweening;
using System;

namespace NFramework.Module.Combat
{

    public enum SpeedType
    {
        Speed,
        Duration,
    }

    public class AbilityItemMoveWithDoTweenComponent : Entity, IAwakeSystem, IUpdateSystem
    {

        public SpeedType speedType;
        public float speed;
        public float duration;

        public TransformComponent transformComponent;
        public TransformComponent targetTransformComponent;

        public Vector3 destination;
        public Tweener moveTweener;
        private Action moveFinishAction;

        public void Awake()
        {
            transformComponent = Parent.GetComponent<TransformComponent>();
        }
        public void Update(float deltaTime)
        {
            if (targetTransformComponent != null)
            {
                if (speedType == SpeedType.Speed)
                {
                    DoMoveToWithSpeed(targetTransformComponent, speed);
                }
                if (speedType == SpeedType.Duration)
                {
                    DoMoveToWithTime(targetTransformComponent, duration);
                }
            }
        }

        public AbilityItemMoveWithDoTweenComponent DoMoveTo(Vector3 destination, float duration)
        {
            this.destination = destination;
            DOTween.To(() => { return targetTransformComponent.Position; }, x => transformComponent.Position = x, destination, duration).SetEase(Ease.Linear);
            return this;
        }

        public void DoMoveToWithSpeed(TransformComponent target, float speed = 1f)
        {
            this.speed = speed;
            speedType = SpeedType.Speed;
            this.targetTransformComponent = target;
            moveTweener?.Kill();
            var dist = Vector3.Distance(targetTransformComponent.Position, targetTransformComponent.Position);
            var duration = dist / speed;
            moveTweener = DOTween.To(() => { return transformComponent.Position; }, x => transformComponent.Position = x, transformComponent.Position, duration);
        }

        public void DoMoveToWithTime(TransformComponent target, float time = 1f)
        {
            duration = time;
            speedType = SpeedType.Duration;
            this.targetTransformComponent = target;
            moveTweener?.Kill();
            moveTweener = DOTween.To(() => { return targetTransformComponent.Position; }, x => targetTransformComponent.Position = x, targetTransformComponent.Position, time);
        }

        public void OnMoveFinish(Action action)
        {
            moveFinishAction = action;
        }

        private void OnMoveFinish()
        {
            moveFinishAction?.Invoke();
        }

    }


}
