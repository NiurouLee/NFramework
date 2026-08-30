

namespace NFramework.ModuleSystem.Combat
{


    public enum AnimationType
    {
        Idle,
        Walk,
        Attack,
        Dead,
    }

    public class AnimationComponent : Entity, IAwakeSystem
    {
        public AnimationType currentType;

        public void PlayAnimation(AnimationType inType, float speed = 1f)
        {
            currentType = inType;
            bool isLoop = currentType == AnimationType.Idle || currentType == AnimationType.Walk ? true : false;
            var syncAnimation = new SyncAnimation(GetParent<CombatEntity>().Id, inType, speed, isLoop);
            GetSystem<EventSystem>().Fire(ref syncAnimation);
        }

        public void Awake()
        {

        }
    }
}
