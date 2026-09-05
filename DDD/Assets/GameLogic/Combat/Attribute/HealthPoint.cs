using NFramework.Core.Live;
using NFramework.Module.EntityModule;

namespace NFramework.Module.Combat
{
    public class HealthPoint : Entity, IAwakeSystem
    {
        public FloatNumeric Current => Parent.GetComponent<AttributeComponent>().HealthPoint;
        public FloatNumeric Max => Parent.GetComponent<AttributeComponent>().HealthPoint;
        public int Value => (int)Current.Value;
        public int MaxValue => (int)Max.Value;

        public void Awake()
        {
            Reset();
        }

        public void Reset()
        {
            Current.BaseValue = MaxValue;
        }

        public void SetMaxValue(int value)
        {
            Max.BaseValue = value;
        }

        public void Minus(int value)
        {
            Current.BaseValue -= value;
        }

        public void Add(int value)
        {
            Current.BaseValue += value;
        }

        public float Percent()
        {
            return (float)Value / MaxValue;
        }

        public bool IsFull()
        {
            return Value == MaxValue;
        }
    }
}