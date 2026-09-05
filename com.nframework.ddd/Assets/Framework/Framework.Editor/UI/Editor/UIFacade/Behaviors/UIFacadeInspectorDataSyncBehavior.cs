using UnityEditor;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 行为：脚本名/ID、ViewConfig 与 JSON 的同步逻辑。
    /// </summary>
    public static class UIFacadeInspectorDataSyncBehavior
    {
        public static void OnFacadeDataChanged(UIFacadeInspectorState state)
        {
            if (state.Facade == null) return;

            GenerateScriptNameAndID(state);
            UpdateViewConfigID(state);
        }

        public static void GenerateScriptNameAndID(UIFacadeInspectorState state)
        {
            if (state.Facade == null) return;

            UIFacadeNameGenerator.GenerateScriptNameAndID(state.Facade, out string scriptName, out string id);
            state.Facade.m_ScriptName = scriptName;
            state.Facade.ID = id;

            EditorUtility.SetDirty(state.Facade);

            if (state.ViewConfig != null && !string.IsNullOrEmpty(state.Facade.m_ScriptName))
            {
                state.ViewConfig.ID = state.Facade.m_ScriptName;
            }
        }

        public static void InitializeViewConfig(UIFacadeInspectorState state)
        {
            if (state.ViewConfig == null) return;

            if (!string.IsNullOrEmpty(state.Facade.m_ScriptName))
            {
                state.ViewConfig.ID = state.Facade.m_ScriptName;
            }

            if (state.ViewConfig.Layer == 0 && !state.ViewConfig.IsWindow)
            {
                state.ViewConfig.SetLayer(0);
                state.ViewConfig.SetWindow(false);
            }
        }

        public static void UpdateViewConfigID(UIFacadeInspectorState state)
        {
            if (state.ViewConfig == null) return;

            if (!string.IsNullOrEmpty(state.Facade.m_ScriptName) && state.ViewConfig.ID != state.Facade.m_ScriptName)
            {
                state.ViewConfig.ID = state.Facade.m_ScriptName;
            }
        }

        public static void LoadViewConfigFromJson(UIFacadeInspectorState state)
        {
            if (state.ViewConfig == null) return;

            string configID = !string.IsNullOrEmpty(state.Facade.m_ScriptName)
                ? state.Facade.m_ScriptName
                : state.Facade.ID;
            if (string.IsNullOrEmpty(configID)) return;

            ViewConfigManager.ViewConfigData data = ViewConfigManager.LoadViewConfig(configID);
            if (data == null) return;

            state.ViewConfig.ID = data.ID;
            state.ViewConfig.AssetID = data.AssetID;
            state.ViewConfig.SetLayer(data.Layer);
            state.ViewConfig.SetWindow(data.IsWindow);
        }
    }
}
