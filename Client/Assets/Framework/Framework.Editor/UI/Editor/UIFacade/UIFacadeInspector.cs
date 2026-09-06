using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// UIFacade 自定义 Inspector：行为由 <see cref="UIFacadeInspectorLifecycleBehavior"/> 等类负责，
    /// 界面由 <see cref="UIFacadeInspectorMainRenderer"/> 按四大区块渲染。
    /// </summary>
    [CustomEditor(typeof(UIFacade))]
    public class UIFacadeInspector : OdinEditor
    {
        private readonly UIFacadeInspectorState _state = new UIFacadeInspectorState();

        internal UIFacadeInspectorState State => _state;

        protected override void OnEnable()
        {
            base.OnEnable();
            UIFacadeInspectorLifecycleBehavior.OnEnable(this);
        }

        protected override void OnDisable()
        {
            UIFacadeInspectorLifecycleBehavior.OnDisable(this);
            base.OnDisable();
        }

        public override void OnInspectorGUI()
        {
            if (target == null) return;

            UIFacadeInspectorLifecycleBehavior.EnsureTargetCurrent(this);
            if (_state.Facade == null) return;

            UIFacadeInspectorMainRenderer.Draw(this);

            if (GUI.changed)
                EditorUtility.SetDirty(_state.Facade);
        }

        /// <summary>
        /// 供渲染区块与 Drawer 回调：数据变更后同步脚本名与 ViewConfig。
        /// </summary>
        internal void HandleDataChanged()
        {
            UIFacadeInspectorDataSyncBehavior.OnFacadeDataChanged(_state);
        }

        /// <summary>
        /// 供外部读取编辑器侧数据（与旧 API 兼容）。
        /// </summary>
        [System.Serializable]
        public class EditorData
        {
            public string ModuleName = "";
            public string SubModuleName = "";
            public string UIName = "";
            public string ScriptName = "";
            public string ID = "";
        }

        public static EditorData GetEditorData(UIFacade facade)
        {
            if (facade == null) return null;

            EditorData data = new EditorData();
            data.ModuleName = facade.m_ModuleName ?? "";
            data.SubModuleName = facade.m_SubModuleName ?? "";
            data.UIName = facade.m_UIName ?? "";
            data.ScriptName = facade.m_ScriptName ?? "";
            data.ID = facade.ID ?? "";
            return data;
        }
    }
}
