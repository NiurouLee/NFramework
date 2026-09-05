using NFramework.Core.Live;
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
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