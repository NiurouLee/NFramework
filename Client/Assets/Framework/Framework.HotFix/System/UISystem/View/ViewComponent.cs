namespace NFramework.ModuleSystem
{
    /// <summary>
    /// ViewComponent 容器：同一 View 上允许保存同类型、不同 Name 的多个 ViewComponent。
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

        /// <summary>按类型获取默认组件；没有默认组件时返回该类型第一个组件（旧兼容行为）</summary>
        public bool TryGetComponent<T>(out T component) where T : ViewComponent
        {
            component = this.ComponentRecord.Get<T>();
            return component != null;
        }

        /// <summary>按类型+Name 获取组件</summary>
        public bool TryGetComponent<T>(string name, out T component) where T : ViewComponent
        {
            component = this.ComponentRecord.Get<T>(name);
            return component != null;
        }

        /// <summary>获取同类型下全部（不同 Name 的）组件</summary>
        public T[] GetComponents<T>() where T : ViewComponent
        {
            return this.ComponentRecord.GetAll<T>();
        }

        public bool TryGetComponents<T>(out T[] components) where T : ViewComponent
        {
            components = this.ComponentRecord.GetAll<T>();
            return components != null && components.Length > 0;
        }

        /// <summary>获取默认组件；没有默认组件时返回该类型第一个组件</summary>
        public T GetComponent<T>() where T : ViewComponent
        {
            return this.ComponentRecord.Get<T>();
        }

        /// <summary>按 Name 获取组件</summary>
        public T GetComponent<T>(string name) where T : ViewComponent
        {
            return this.ComponentRecord.Get<T>(name);
        }

        /// <summary>获取或创建默认组件（同类型已存在则复用）</summary>
        public T AddComponent<T>() where T : ViewComponent, new()
        {
            return this.ComponentRecord.Add<T>();
        }

        /// <summary>获取或创建指定 Name 的组件（同类型同 Name 已存在则复用）</summary>
        public T AddComponent<T>(string name) where T : ViewComponent, new()
        {
            return this.ComponentRecord.Add<T>(name);
        }

        /// <summary>注册一个外部创建的组件实例，Name 为空时作为默认组件</summary>
        public T AddComponent<T>(T inComponent, string name = null) where T : ViewComponent
        {
            return this.ComponentRecord.Add(inComponent, name);
        }

        public bool HasComponent<T>() where T : ViewComponent
        {
            return this.ComponentRecord.Has<T>();
        }

        /// <summary>判断某类型下是否存在指定 Name 的组件</summary>
        public bool HasComponent<T>(string name) where T : ViewComponent
        {
            return this.ComponentRecord.Has<T>(name);
        }

        /// <summary>按 Name 移除组件（只移除记录，不调用 Destroy）</summary>
        public bool RemoveComponent<T>(string name) where T : ViewComponent
        {
            return this.ComponentRecord.TryRemove<T>(name);
        }

        /// <summary>移除指定组件实例（只移除记录，不调用 Destroy）</summary>
        public bool RemoveComponent<T>(T inComponent) where T : ViewComponent
        {
            return this.ComponentRecord.TryRemove(inComponent);
        }

        /// <summary>移除某类型下全部组件</summary>
        public bool RemoveAllComponent<T>() where T : ViewComponent
        {
            return this.ComponentRecord.RemoveAll<T>();
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
