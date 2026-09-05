using NFramework.Core.Live;
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public class CombatTagComponent : Entity, IAwakeSystem<CombatTagType>
    {
        public CombatTagType tagType { get; private set; }

        public void Awake(CombatTagType a)
        {
            tagType = a;
        }
    }
}