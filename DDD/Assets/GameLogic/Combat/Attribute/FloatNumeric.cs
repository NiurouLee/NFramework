using NFramework.Module.EventModule;
using NFramework.Module.EntityModule;
using NFramework.Core.Live;

namespace NFramework.Module.Combat
{
    public class FloatNumeric : Entity, IAwakeSystem<NumericEntity, AttributeType>
    {
        public NumericEntity m_NumericEntity;
        private int m_Type;
        public float BaseValue
        {
            get
            {
                return m_NumericEntity.GetFloat(m_Type * 10 + 1);
            }
            set
            {
                m_NumericEntity.Set(m_Type * 10 + 1, value);
                var syncAttribute = new SyncModifyAttribute(Parent.Id, m_Type, 1, value);
                NFROOT.Instance.GetModule<EventM>().D.Fire(ref syncAttribute);
            }
        }

        public float Add
        {
            get
            {
                return m_NumericEntity.GetFloat(m_Type * 10 + 2);

            }
            set
            {
                m_NumericEntity.Set(m_Type * 10 + 2, value);
                var syncAttribute = new SyncModifyAttribute(Parent.Id, m_Type, 2, value);
                NFROOT.Instance.GetModule<EventM>().D.Fire(ref syncAttribute);
            }
        }

        public float PctAdd
        {
            get
            {
                return m_NumericEntity.GetFloat(m_Type * 10 + 3);
            }
            set
            {
                m_NumericEntity.Set(m_Type * 10 + 3, value);
                var syncAttribute = new SyncModifyAttribute(Parent.Id, m_Type, 3, value);
                NFROOT.Instance.GetModule<EventM>().D.Fire(ref syncAttribute);
            }
        }

        public float FinalAdd
        {
            get
            {
                return m_NumericEntity.GetFloat(m_Type * 10 + 4);
            }
            set
            {
                m_NumericEntity.Set(m_Type * 10 + 4, value);
                var syncAttribute = new SyncModifyAttribute(Parent.Id, m_Type, 4, value);
                NFROOT.Instance.GetModule<EventM>().D.Fire(ref syncAttribute);
            }
        }

        public float FinalPctAdd
        {
            get
            {
                return m_NumericEntity.GetFloat(m_Type * 10 + 5);
            }
            set
            {
                m_NumericEntity.Set(m_Type * 10 + 5, value);
                var syncAttribute = new SyncModifyAttribute(Parent.Id, m_Type, 5, value);
                NFROOT.Instance.GetModule<EventM>().D.Fire(ref syncAttribute);
            }
        }

        public float Value => m_NumericEntity.GetFloat(m_Type);

        public void Awake(NumericEntity a, AttributeType b)
        {
            m_NumericEntity = a;
            m_Type = (int)(AttributeType)b;
        }
    }

}