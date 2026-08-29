using    NFramework.ModuleSystem;
using   NFramework.ModuleSystem;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// ViewComponent
    /// </summary>
    public partial class View : NUIObject
    {
        private ViewComponentRecords m_componentRecords;
        private ViewComponentRecords ComponentRecord
        {
            get
            {
                if (m_componentRecords == null)
                {
                    m_componentRecords = this.GetSystem<ObjectPoolSystem>().Alloc<ViewComponentRecords>();
                    m_componentRecords.Awake(this);
                }
                return m_componentRecords;
            }
        }

        public bool TryGetComponent<T>(out T component) where T : ViewComponent
        {
            component = this.ComponentRecord.Get<T>();
            return component != null;
        }
        
        public bool TryGetComponent<T>(string name,out T component) where T : ViewComponent
        {
            component = this.ComponentRecord.Get<T>(name);
            return component != null;
        }

        public T AddComponent<T>() where T : ViewComponent, new()
        {
            return this.ComponentRecord.Add<T>();
        }

        public T AddComponent<T>(string name) where T : ViewComponent, new()
        {
            return this.ComponentRecord.Add<T>(name);
        }

        public bool HasComponent<T>() where T : ViewComponent
        {
            return this.ComponentRecord.Has<T>();
        }

        public T GetComponent<T>() where T : ViewComponent
        {
            var component = this.ComponentRecord.Get<T>();
            return component;
        }
        
        public T GetComponent<T>(string name) where T : ViewComponent
        {
            var component = this.ComponentRecord.Get<T>(name);
            return component;
        }

        public void DestroyComponentContainer()
        {
            if (m_componentRecords != null)
            {
                m_componentRecords.Destroy();
                this.GetSystem<ObjectPoolSystem>().Free(m_componentRecords);
                m_componentRecords = null;
            }
        }

    }
}
