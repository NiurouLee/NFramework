using UnityEditor;
using UnityEngine;
using System.IO;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 行为：ViewConfig管理逻辑
    /// </summary>
    public static class UIFacadeInspectorViewConfigBehavior
    {
        /// <summary>
        /// 更新ViewConfig的ID
        /// </summary>
        public static void UpdateViewConfigID(UIFacade facade, ViewConfig viewConfig)
        {
            if (viewConfig == null) return;

            if (!string.IsNullOrEmpty(facade.m_ScriptName))
            {
                if (viewConfig.ID != facade.m_ScriptName)
                {
                    viewConfig.ID = facade.m_ScriptName;
                }
            }
        }

        /// <summary>
        /// 更新ViewConfig的AssetID
        /// </summary>
        public static void UpdateViewConfigAssetID(UIFacade facade, ViewConfig viewConfig)
        {
            if (viewConfig == null || facade == null) return;

            // 获取 prefab 路径
            string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(facade.gameObject);
            if (string.IsNullOrEmpty(prefabPath))
            {
                var prefabInstance = PrefabUtility.GetCorrespondingObjectFromSource(facade.gameObject);
                if (prefabInstance != null)
                {
                    prefabPath = AssetDatabase.GetAssetPath(prefabInstance);
                }
            }

            if (!string.IsNullOrEmpty(prefabPath))
            {
                string prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                if (string.IsNullOrEmpty(viewConfig.AssetID) || viewConfig.AssetID != prefabName)
                {
                    viewConfig.AssetID = prefabName;
                }
            }
        }

        /// <summary>
        /// 同步ID到ViewConfig
        /// </summary>
        public static void SyncIDToViewConfig(UIFacade facade, ViewConfig viewConfig)
        {
            if (!string.IsNullOrEmpty(facade.m_ScriptName))
            {
                UpdateViewConfigID(facade, viewConfig);
                EditorUtility.SetDirty(facade);
                Debug.Log($"已同步配置ID: {facade.m_ScriptName}");
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "脚本名称为空，无法同步。请先在\"基本信息\"中填写模块名称和UI名称。", "确定");
            }
        }

        /// <summary>
        /// 保存ViewConfig到JSON
        /// </summary>
        public static void SaveViewConfigToJson(UIFacade facade, ViewConfig viewConfig)
        {
            if (string.IsNullOrEmpty(facade.m_ScriptName))
            {
                EditorUtility.DisplayDialog("错误",
                    "脚本名称为空，无法保存ViewConfig！\n\n请先在\"基本信息\"中填写：\n- 模块名称\n- UI名称\n\n系统会自动生成脚本名称。", "确定");
                return;
            }

            // 确保ViewConfig的ID是最新的脚本名称
            UpdateViewConfigID(facade, viewConfig);

            // 更新AssetID
            UpdateViewConfigAssetID(facade, viewConfig);

            // 保存ViewConfig
            if (!ViewConfigManager.SaveViewConfig(viewConfig, facade.m_ScriptName, viewConfig.AssetID,
                    out string errorMessage))
            {
                EditorUtility.DisplayDialog("错误", errorMessage, "确定");
                return;
            }

            // 生成ViewTypeRegistry映射表
            if (!ViewTypeRegistryGenerator.GenerateRegistryClass(out string genErrorMessage))
            {
                EditorUtility.DisplayDialog("警告",
                    $"ViewConfig已保存，但生成类型注册表失败：\n{genErrorMessage}",
                    "确定");
                return;
            }

            Debug.Log($"ViewConfig已保存: {facade.m_ScriptName}");
        }

    }
}
