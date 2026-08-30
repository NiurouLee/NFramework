

namespace NFramework.ModuleSystem.Combat
{
    public class AABBComponent : Entity, IAwakeSystem<AABB>
    {
        public AABB aabb;

        public void Awake(AABB a)
        {
            aabb = a;
        }
    }
}