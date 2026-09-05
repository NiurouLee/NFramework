using NFramework.Core.Live;
using NFramework.Module.EntityModule;
using NFramework.Module.Math;

namespace NFramework.Module.Combat
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