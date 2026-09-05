using NFramework.Module.EventModule;
using UnityEngine;


namespace NFramework.Module.Combat
{
    public struct SyncCreateCombat : IEvent
    {
        public long id;
        public SyncCreateCombat(long id)
        {
            this.id = id;
        }
    }
    public struct SyncDeleteCombat : IEvent
    {
        public long id;
        public SyncDeleteCombat(long id)
        {
            this.id = id;
        }
    }

    public struct SyncCreateAbilityItem : IEvent
    {
        public long id;
        public SyncCreateAbilityItem(long id)
        {
            this.id = id;
        }
    }

    public class SyncDeleteAbilityItem
    {
        public long id;
        public SyncDeleteAbilityItem(long id)
        {
            this.id = id;
        }
    }

    public struct SyncTransform : IEvent
    {
        public long id;
        public Vector3 Position;
        public Quaternion rotation;
        public Vector3 localScale;

        public SyncTransform(long id, Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            this.id = id;
            Position = position;
            this.rotation = rotation;
            this.localScale = localScale;
        }
    }

    public struct SyncAnimation : IEvent
    {
        public long id;
        public AnimationType animationType;
        public float speed;
        public bool isLoop;

        public SyncAnimation(long id, AnimationType animationType, float speed, bool isLoop)
        {
            this.id = id;
            this.animationType = animationType;
            this.speed = speed;
            this.isLoop = isLoop;
        }
    }

    public struct SyncParticleEffect : IEvent
    {
        public long id;
        public string name;
        public UnityEngine.Vector3 position;
        public UnityEngine.Quaternion rotation;

        public SyncParticleEffect(long id, string name, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation)
        {
            this.id = id;
            this.name = name;
            this.position = position;
            this.rotation = rotation;
        }
    }

    public struct SyncDeleteParticleEffect : IEvent
    {
        public long id;
        public string name;

        public SyncDeleteParticleEffect(long id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    public struct SyncDamage : IEvent
    {
        public long id;

        public int damage;

        public SyncDamage(long id, int damage)
        {
            this.id = id;
            this.damage = damage;
        }
    }

    public struct SyncCure : IEvent
    {
        public long id;
        public int cure;

        public SyncCure(long id, int cure)
        {
            this.id = id;
            this.cure = cure;
        }
    }

    public struct SyncAttribute : IEvent
    {
        public long id;
        public AttributeType attributeType;

        public SyncAttribute(long id, AttributeType attributeType)
        {
            this.id = id;
            this.attributeType = attributeType;
        }
    }

    public class SyncModifyAttribute : IEvent
    {
        public long id;
        public int type;
        public int temp;
        public float value;

        public SyncModifyAttribute(long id, int type, int temp, float value)
        {
            this.id = id;
            this.type = type;
            this.temp = temp;
            this.value = value;
        }
    }
}
