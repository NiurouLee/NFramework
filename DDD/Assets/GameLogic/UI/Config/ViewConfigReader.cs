using System.Collections.Generic;
using UnityEngine;
using NFramework.ModuleSystem;
using System.IO;

using XHFramework;
namespace Game.Logic
{
    /// <summary>
    /// 把读取ui 相关的配置的东西全部放到这里，框架直接接受传入
    /// </summary>
    public class ViewConfigReader
    {
        [System.Serializable]
        private class ViewConfigsContainer
        {
            public List<ViewConfig> Configs = new List<ViewConfig>();
        }

        public Dictionary<string, ViewConfig> ConfigMap { get; private set; }
        public Dictionary<System.Type, string> Type2ConfigIDMap { get; private set; }


        private bool s_Initialized = false;

        /// <summary>
        /// 初始化，从JSON文件加载所有ViewConfig到字典中
        /// </summary>
        public void Initialize()
        {
            if (s_Initialized) return;

            ConfigMap = new Dictionary<string, ViewConfig>();
            LoadFromJson();
            s_Initialized = true;
        }

        /// <summary>
        /// 从JSON文件加载配置
        /// </summary>
        private void LoadFromJson()
        {
            string jsonPath = NFROOT.Instance.Config.UISystemConfig.ViewConfigsJsonPath;
            if (File.Exists(jsonPath))
            {
                try
                {
                    string json = File.ReadAllText(jsonPath);
                    LoadFromJsonString(json);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"加载ViewConfigs.json失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 从JSON字符串加载配置
        /// </summary>
        private void LoadFromJsonString(string json)
        {
            try
            {
                ViewConfigsContainer container = JsonUtility.FromJson<ViewConfigsContainer>(json);
                if (container != null && container.Configs != null)
                {
                    foreach (var data in container.Configs)
                    {
                        if (string.IsNullOrEmpty(data.ID)) continue;

                        ViewConfig config = new ViewConfig();
                        config.ID = data.ID;
                        config.AssetID = data.AssetID;
                        config.SetLayer(data.Layer);
                        config.SetWindow(data.IsWindow);
                        config.SetFixedLayer(data.IsFixedLayer);
                        this.ConfigMap[data.ID] = config;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"解析ViewConfigs.json失败: {ex.Message}");
            }
        }
    }
}