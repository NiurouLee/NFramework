
using NFramework.ModuleSystem;

namespace NFramework.ModuleSystem.Combat
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