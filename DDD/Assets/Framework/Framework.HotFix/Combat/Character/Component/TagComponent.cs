
using NFramework.ModuleSystem;

namespace NFramework.ModuleSystem.Combat
{
    public enum TagType
    {
        Player,
        Friend,
        Enemy,
    }

    public class TagComponent: Entity, IAwakeSystem<TagType>
    {
        public TagType tagType;
        public void Awake(TagType inTagType)
        {
            tagType = inTagType;
        }
    }
}