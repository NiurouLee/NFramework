using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

using NFramework;
namespace NFramework.ModuleSystem
{
    /// <summary>
    /// ViewConfig管理器，负责ViewConfig的保存、加载、验证
    /// </summary>
    public static class ViewConfigManager
    {
        [System.Serializable]
        public class ViewConfigData
        {
            public string ID;
            public string Namespace;
            public string AssetID;
            public ushort Layer;
            public bool IsWindow;
            public bool IsFixedLayer;
        }

        [System.Serializable]
        public class ViewConfigsContainer
        {
            public List<ViewConfigData> Configs = new List<ViewConfigData>();
        }

        /// <summary>
        /// 加载所有ViewConfig
        /// </summary>
        public static ViewConfigsContainer LoadAllViewConfigs()
        {
            string jsonFilePath = GetJsonFilePath();
            ViewConfigsContainer container = new ViewConfigsContainer();
            container.Configs = new List<ViewConfigData>();

            if (File.Exists(jsonFilePath))
            {
                try
                {
                    string json = File.ReadAllText(jsonFilePath);
                    container = JsonUtility.FromJson<ViewConfigsContainer>(json);
                    if (container == null || container.Configs == null)
                    {
                        container = new ViewConfigsContainer();
                        container.Configs = new List<ViewConfigData>();
                    }
                }
                catch
                {
                    container = new ViewConfigsContainer();
                    container.Configs = new List<ViewConfigData>();
                }
            }

            return container;
        }

        /// <summary>
        /// 根据ID加载ViewConfig
        /// </summary>
        public static ViewConfigData LoadViewConfig(string configID)
        {
            if (string.IsNullOrEmpty(configID)) return null;

            ViewConfigsContainer container = LoadAllViewConfigs();
            foreach (var config in container.Configs)
            {
                if (config.ID == configID)
                {
                    return config;
                }
            }

            return null;
        }

        /// <summary>
        /// 保存ViewConfig
        /// </summary>
        public static bool SaveViewConfig(ViewConfig viewConfig, string configID, string assetID, out string errorMessage)
        {
            errorMessage = "";

            if (viewConfig == null)
            {
                errorMessage = "ViewConfig未初始化";
                return false;
            }

            if (string.IsNullOrEmpty(configID))
            {
                errorMessage = "配置ID不能为空";
                return false;
            }

            string jsonFolderPath = FrameworkConfigAssetEditor.GetConfig().UISystemConfig.ViewConfigDataRoot;
            if (!Directory.Exists(jsonFolderPath)) Directory.CreateDirectory(jsonFolderPath);

            string jsonFilePath = GetJsonFilePath();
            ViewConfigsContainer container = LoadAllViewConfigs();

            // 检查并清理重复项
            CleanDuplicateConfigs(container);

            // 检查是否是更新现有配置
            bool isUpdatingExisting = false;
            foreach (var existingConfig in container.Configs)
            {
                if (existingConfig.ID == configID)
                {
                    isUpdatingExisting = true;
                    break;
                }
            }

            // 如果不是更新现有配置，检查是否有其他配置使用了相同的ID
            // 注意：这里不需要排除当前配置，因为 isUpdatingExisting 为 false 时，当前配置还不存在
            if (!isUpdatingExisting)
            {
                if (!CheckConfigIDDuplicate(configID, out string duplicateInfo, null))
                {
                    errorMessage = $"配置ID '{configID}' 已存在！\n\n{duplicateInfo}\n\n请修改UI名称或模块名称以生成不同的脚本名称。";
                    return false;
                }
            }

            // 创建配置数据
            var configData = new ViewConfigData
            {
                ID = configID,
                AssetID = assetID ?? "",
                Layer = viewConfig.Layer,
                IsWindow = viewConfig.IsWindow,
                IsFixedLayer = viewConfig.IsFixedLayer
            };

            // 更新或添加配置
            bool found = false;
            for (int i = 0; i < container.Configs.Count; i++)
            {
                if (container.Configs[i].ID == configID)
                {
                    container.Configs[i] = configData;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                container.Configs.Add(configData);
            }

            // 最终检查：确保保存后没有重复的ID
            if (HasDuplicateIDs(container.Configs, out List<string> duplicates))
            {
                errorMessage = $"保存后检测到重复的配置ID，保存已取消！\n\n重复的ID:\n{string.Join("\n", duplicates)}";
                return false;
            }

            // 保存到JSON文件
            try
            {
                string json = JsonUtility.ToJson(container, true);
                File.WriteAllText(jsonFilePath, json);
                AssetDatabase.Refresh();
                return true;
            }
            catch (System.Exception ex)
            {
                errorMessage = $"保存失败: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// 检查配置ID是否重复
        /// </summary>
        /// <param name="configID">要检查的配置ID</param>
        /// <param name="duplicateInfo">重复信息</param>
        /// <param name="excludeConfigID">排除的配置ID（通常是当前正在编辑的配置，避免误报）</param>
        public static bool CheckConfigIDDuplicate(string configID, out string duplicateInfo, string excludeConfigID = null)
        {
            duplicateInfo = "";
            if (string.IsNullOrEmpty(configID)) return false;

            ViewConfigsContainer container = LoadAllViewConfigs();
            int count = 0;
            List<string> duplicateIDs = new List<string>();

            foreach (var config in container.Configs)
            {
                // 排除当前配置本身
                if (!string.IsNullOrEmpty(excludeConfigID) && config.ID == excludeConfigID)
                {
                    continue;
                }

                if (config.ID == configID)
                {
                    count++;
                    duplicateIDs.Add(config.ID);
                }
            }

            if (count > 0)
            {
                duplicateInfo = $"已存在 {count} 个相同ID的配置";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检查是否有重复的ID
        /// </summary>
        public static bool HasDuplicateIDs(List<ViewConfigData> configs, out List<string> duplicateIDs)
        {
            duplicateIDs = new List<string>();
            HashSet<string> idSet = new HashSet<string>();

            foreach (var config in configs)
            {
                if (string.IsNullOrEmpty(config.ID)) continue;

                if (idSet.Contains(config.ID))
                {
                    if (!duplicateIDs.Contains(config.ID))
                    {
                        duplicateIDs.Add(config.ID);
                    }
                }
                else
                {
                    idSet.Add(config.ID);
                }
            }

            return duplicateIDs.Count > 0;
        }

        /// <summary>
        /// 清理重复的配置
        /// </summary>
        public static void CleanDuplicateConfigs(ViewConfigsContainer container)
        {
            if (container == null || container.Configs == null) return;

            HashSet<string> seenIDs = new HashSet<string>();
            List<ViewConfigData> cleanedConfigs = new List<ViewConfigData>();

            foreach (var config in container.Configs)
            {
                if (string.IsNullOrEmpty(config.ID)) continue;

                if (!seenIDs.Contains(config.ID))
                {
                    seenIDs.Add(config.ID);
                    cleanedConfigs.Add(config);
                }
            }

            container.Configs = cleanedConfigs;
        }

        /// <summary>
        /// 检查层级是否与其他ViewConfig重复
        /// </summary>
        public static string CheckLayerDuplicate(ushort layer, string currentConfigID)
        {
            ViewConfigsContainer container = LoadAllViewConfigs();

            foreach (var data in container.Configs)
            {
                if (data.ID == currentConfigID) continue;

                if (data.IsFixedLayer && data.Layer == layer)
                {
                    return data.ID;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取层级冲突信息
        /// </summary>
        public static string GetLayerConflictInfo(ushort layer, string currentConfigID)
        {
            List<string> conflicts = new List<string>();
            ViewConfigsContainer container = LoadAllViewConfigs();

            foreach (var data in container.Configs)
            {
                if (data.ID == currentConfigID) continue;

                if (data.IsFixedLayer && data.Layer == layer)
                {
                    conflicts.Add(data.ID);
                }
            }

            if (conflicts.Count > 0)
            {
                return string.Join(", ", conflicts);
            }

            return null;
        }

        /// <summary>
        /// 运行时加载所有ViewConfig到字典中
        /// </summary>
        public static Dictionary<string, ViewConfig> LoadAllViewConfigsToMap()
        {
            Dictionary<string, ViewConfig> configMap = new Dictionary<string, ViewConfig>();
            
            ViewConfigsContainer container = LoadAllViewConfigs();
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
                    
                    configMap[data.ID] = config;
                }
            }
            
            return configMap;
        }

        private static string GetJsonFilePath()
        {
            return FrameworkConfigAssetEditor.GetConfig().UISystemConfig.ViewConfigsJsonPath;
        }
    }
}