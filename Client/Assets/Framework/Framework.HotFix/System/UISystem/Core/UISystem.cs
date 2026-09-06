using System; using System.Collections.Generic; 
namespace NFramework.ModuleSystem
{
    public partial class UISystem : FrameworkSystemModuleBase
    {
        private ViewConfigServices m_ConfigServices { get; set; }

        public void AwakeConfigServices()
        {
            this.m_ConfigServices = new ViewConfigServices();
        }

        /// <summary>
        /// </summary>
        /// 设置viewConfig, 这一步是必须的，不然UI系统不会正常运行
        public void RegisterConfig(Dictionary<string, ViewConfig> inConfigMap,
            Dictionary<System.Type, string> inType2ConfigIDMap)
        {
            foreach (var kv in inType2ConfigIDMap)
            {
                var type = kv.Key;
                var configID = kv.Value;
                var config = inConfigMap[configID];
                this.m_ConfigServices.AddViewConfig(type, configID, config);
            }
        }

        public ViewConfig GetViewConfig<T>(T inView) where T : View
        {
            return this.m_ConfigServices.GetViewConfig(inView);
        }

        public ViewConfig GetViewConfig<T>() where T : View
        {
            return this.m_ConfigServices.GetViewConfig<T>();
        }

        public ViewConfig GetViewConfig(string inID)
        {
            return this.m_ConfigServices.GetViewConfig(inID);
        }

        public string GetViewID<T>(T inView) where T : View
        {
            return this.m_ConfigServices.GetViewConfig(inView).ID;
        }

        public string GetViewID(System.Type inType)
        {
            return this.m_ConfigServices.GetViewConfigByType(inType)?.ID;
        }

        public string GetViewID<T>() where T : View
        {
            return this.m_ConfigServices.GetViewConfig<T>().ID;
        }

        public T CreateView<T>() where T : View, new()
        {
            return new T();
        }

        public View CreateView(ViewConfig inViewConfig)
        {
            var type = this.m_ConfigServices.GetViewType(inViewConfig.ID);
            if (type == null)
            {
                throw new Exception($"ViewConfig {inViewConfig.ID} not found");
            }

            return (View)Activator.CreateInstance(type);
        }

        public string MappingAssetID(string inAssetID)
        {
            return inAssetID;
        }
    }
}