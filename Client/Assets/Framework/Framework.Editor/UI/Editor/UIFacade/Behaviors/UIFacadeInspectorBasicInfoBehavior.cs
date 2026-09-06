using UnityEditor;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 行为：基本信息管理逻辑
    /// </summary>
    public static class UIFacadeInspectorBasicInfoBehavior
    {
        /// <summary>
        /// 处理子模块切换
        /// </summary>
        public static void HandleSubModuleToggle(UIFacade facade, bool newEnableSubModule, System.Action onDataChanged)
        {
            if (facade.m_EnableSubModule == newEnableSubModule) return;

            facade.m_EnableSubModule = newEnableSubModule;

            if (!facade.m_EnableSubModule && !string.IsNullOrEmpty(facade.m_SubModuleName))
            {
                if (EditorUtility.DisplayDialog("确认", "禁用子模块将清空子模块名称，确定继续吗？", "确定", "取消"))
                {
                    facade.m_SubModuleName = "";
                    GenerateScriptNameAndID(facade);
                    onDataChanged?.Invoke();
                }
                else
                {
                    facade.m_EnableSubModule = true;
                }
            }
            else
            {
                GenerateScriptNameAndID(facade);
                onDataChanged?.Invoke();
            }

            EditorUtility.SetDirty(facade);
        }

        /// <summary>
        /// 处理名称字段变更
        /// </summary>
        public static void HandleNameFieldChanged(UIFacade facade, System.Action onDataChanged)
        {
            GenerateScriptNameAndID(facade);
            EditorUtility.SetDirty(facade);
            onDataChanged?.Invoke();
        }

        /// <summary>
        /// 重新生成脚本名称和ID
        /// </summary>
        public static void GenerateScriptNameAndID(UIFacade facade)
        {
            UIFacadeNameGenerator.GenerateScriptNameAndID(facade, out string scriptName, out string id);
            facade.m_ScriptName = scriptName;
            facade.ID = id;
        }

        /// <summary>
        /// 检查脚本名称是否重复
        /// </summary>
        public static bool CheckScriptNameDuplicate(UIFacade facade, out string duplicateInfo)
        {
            duplicateInfo = "";

            if (string.IsNullOrEmpty(facade.m_ScriptName))
                return true;

            // 检查当前配置是否已经在JSON中保存过
            string excludeConfigID = null;
            ViewConfigManager.ViewConfigData existingConfig = ViewConfigManager.LoadViewConfig(facade.m_ScriptName);
            if (existingConfig != null && existingConfig.ID == facade.m_ScriptName)
            {
                excludeConfigID = facade.m_ScriptName;
            }

            return UIFacadeNameGenerator.CheckScriptNameDuplicate(facade.m_ScriptName, out duplicateInfo, excludeConfigID);
        }

        /// <summary>
        /// 复制ID到剪贴板
        /// </summary>
        public static void CopyIDToClipboard(UIFacade facade)
        {
            if (!string.IsNullOrEmpty(facade.ID))
            {
                EditorGUIUtility.systemCopyBuffer = facade.ID;
                Debug.Log($"已复制ID: {facade.ID}");
            }
        }
    }
}