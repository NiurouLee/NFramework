using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 渲染：按四大区块绘制（逻辑与原先一致，仅集中入口）。
    /// 1. 基本信息 2. UI元素列表 3. View配置 4. 工具栏
    /// </summary>
    public static class UIFacadeInspectorMainRenderer
    {
        public static void Draw(UIFacadeInspector inspector)
        {
            var state = inspector.State;
            if (state.Facade == null) return;

            SirenixEditorGUI.Title("UI Facade Configuration", "配置UI门面组件", TextAlignment.Center, true);
            EditorGUILayout.Space(5);

            // 区块一：基本信息
            UIFacadeBasicInfoDrawer.DrawBasicInfo(state.Facade, ref state.FoldBasicInfo, inspector.HandleDataChanged);
            EditorGUILayout.Space(2);

            // 拖放区：拖入 GameObject 自动添加 UI 元素
            UIFacadeUIElementsDrawer.DrawGameObjectDropArea(state.Facade, inspector.HandleDataChanged);
            EditorGUILayout.Space(2);

            // 区块二：UI 元素列表
            UIFacadeUIElementsDrawer.DrawUIElementsList(state.Facade, ref state.FoldUIElementsList, inspector.HandleDataChanged);
            EditorGUILayout.Space(2);

            // 区块三：View 配置
            UIFacadeViewConfigDrawer.DrawViewConfig(state.Facade, state.ViewConfig, ref state.FoldViewConfig);
            EditorGUILayout.Space(2);

            // 区块四：工具栏
            UIFacadeToolsDrawer.DrawToolButtons(state.Facade, ref state.FoldTools, inspector.HandleDataChanged);
        }
    }
}
