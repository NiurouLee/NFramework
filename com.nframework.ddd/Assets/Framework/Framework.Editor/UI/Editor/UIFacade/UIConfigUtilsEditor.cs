using System.Collections.Generic;
using    NFramework.ModuleSystem;
using Unity.VisualScripting;

using NFramework;
namespace NFramework.ModuleSystem
{
    /// <summary>
    /// UI配置工具类编辑器，提供ViewConfig的管理和加载功能
    /// </summary>
    public class UIConfigUtilsEditor
    {
        // 使用GameObject的InstanceID作为key，确保每个UIFacade实例都有独立的ViewConfig
        public static Dictionary<int, ViewConfig> m_ViewConfigDic = new Dictionary<int, ViewConfig>();
        
        /// <summary>
        /// 获取指定UIFacade的ViewConfig
        /// </summary>
        /// <param name="target">目标UIFacade对象</param>
        /// <returns>对应的ViewConfig实例</returns>
        public static ViewConfig GetViewConfig(UnityEngine.Object target)
        {
            UIFacade _uiFacade = (UIFacade)target;
            
            // 使用GameObject的InstanceID作为key，确保每个实例都有独立的ViewConfig
            int instanceID = _uiFacade.gameObject.GetInstanceID();
            
            if (m_ViewConfigDic.TryGetValue(instanceID, out var config))
            {
                return config;
            }

            // 创建新的ViewConfig实例
            var _config = new ViewConfig();
            
            // 尝试从JSON文件加载配置
            LoadViewConfigFromJson(_config, _uiFacade);
            
            m_ViewConfigDic.Add(instanceID, _config);
            return _config;
        }

        /// <summary>
        /// 从JSON文件加载ViewConfig配置
        /// </summary>
        private static void LoadViewConfigFromJson(ViewConfig config, UIFacade uiFacade)
        {
            // 先设置默认值
            config.ID = uiFacade.ID;
            
            // 尝试从JSON文件加载
            string jsonFilePath = FrameworkConfigAssetEditor.GetConfig().UISystemConfig.ViewConfigsJsonPath;
            
            if (System.IO.File.Exists(jsonFilePath))
            {
                try
                {
                    string json = System.IO.File.ReadAllText(jsonFilePath);
                    ViewConfigsContainer container = UnityEngine.JsonUtility.FromJson<ViewConfigsContainer>(json);
                    
                    if (container != null && container.Configs != null)
                    {
                        // 查找匹配的配置（使用脚本名称或ID）
                        string configID = uiFacade.ID;
                        foreach (var data in container.Configs)
                        {
                            if (data.ID == configID)
                            {
                                config.ID = data.ID;
                                config.AssetID = data.AssetID;
                                config.SetLayer(data.Layer);
                                config.SetWindow(data.IsWindow);
                                return;
                            }
                        }
                    }
                }
                catch
                {
                    // 加载失败，使用默认值
                }
            }
        }

        [System.Serializable]
        private class ViewConfigData
        {
            public string ID;
            public string AssetID;
            public ushort Layer;
            public bool IsWindow;
        }

        [System.Serializable]
        private class ViewConfigsContainer
        {
            public System.Collections.Generic.List<ViewConfigData> Configs = new System.Collections.Generic.List<ViewConfigData>();
        }

        public static void SaveViewConfig(UnityEngine.Object target)
        {
            UIFacade _uiFacade = (UIFacade)target;
        }
    }
}
