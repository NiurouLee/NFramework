using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.IO;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// UIFacade ViewConfig绘制器
    /// </summary>
    public static class UIFacadeViewConfigDrawer
    {
        /// <summary>
        /// 绘制ViewConfig配置区域
        /// </summary>
        public static void DrawViewConfig(UIFacade facade, ViewConfig viewConfig, ref bool foldout)
        {
            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();
            foldout = EditorGUILayout.Foldout(foldout, "View配置", true);
            SirenixEditorGUI.EndBoxHeader();

            if (foldout)
            {
                if (viewConfig == null)
                {
                    viewConfig = UIConfigUtilsEditor.GetViewConfig(facade);
                    UIFacadeInspectorViewConfigBehavior.UpdateViewConfigAssetID(facade, viewConfig);

                    if (viewConfig == null)
                    {
                        SirenixEditorGUI.ErrorMessageBox("ViewConfig初始化失败！");
                        SirenixEditorGUI.EndBox();
                        return;
                    }
                }

                // 确保ViewConfig.ID始终使用最新的脚本名称
                UIFacadeInspectorViewConfigBehavior.UpdateViewConfigID(facade, viewConfig);

                // 配置说明
                SirenixEditorGUI.InfoMessageBox("配置UI视图的显示层级和窗口属性");

                EditorGUILayout.Space(5);

                EditorGUI.BeginChangeCheck();

                // ID字段（只读显示，使用脚本名称）
                DrawConfigID(facade);

                // AssetID字段（只读，使用prefab名字）
                DrawAssetID(facade, viewConfig);

                EditorGUILayout.Space(8);

                // 固定层级设置（可编辑）
                DrawFixedLayer(facade, viewConfig);

                EditorGUILayout.Space(5);

                // UI层级设置（始终可编辑）
                DrawUILayer(facade, viewConfig);

                EditorGUILayout.Space(5);

                // 窗口模式设置（可编辑）
                DrawWindowMode(viewConfig);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(facade);
                }

                EditorGUILayout.Space(5);

                // ViewConfig工具按钮
                DrawViewConfigTools(facade, viewConfig);
            }

            SirenixEditorGUI.EndBox();
        }

        private static void DrawConfigID(UIFacade facade)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("配置ID:", GUILayout.Width(80));
                string configID = !string.IsNullOrEmpty(facade.m_ScriptName) ? facade.m_ScriptName : "未设置（请先填写脚本名称）";
                EditorGUILayout.LabelField(configID, EditorStyles.label, GUILayout.MinWidth(120));
            }
            EditorGUILayout.EndHorizontal();

            // 如果脚本名称为空，显示警告
            if (string.IsNullOrEmpty(facade.m_ScriptName))
            {
                EditorGUILayout.HelpBox("⚠ 警告：脚本名称为空，无法保存ViewConfig。请先在\"基本信息\"中填写模块名称和UI名称。", MessageType.Warning);
            }

            EditorGUILayout.Space(2);
        }

        private static void DrawAssetID(UIFacade facade, ViewConfig viewConfig)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("资源ID:", GUILayout.Width(80));
                UIFacadeInspectorViewConfigBehavior.UpdateViewConfigAssetID(facade, viewConfig);
                string displayAssetID = !string.IsNullOrEmpty(viewConfig.AssetID) ? viewConfig.AssetID : "未设置";
                EditorGUILayout.LabelField(displayAssetID, EditorStyles.label, GUILayout.MinWidth(120));
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawFixedLayer(UIFacade facade, ViewConfig viewConfig)
        {
            bool currentIsFixedLayer = viewConfig.IsFixedLayer;
            bool newIsFixedLayer = EditorGUILayout.Toggle("是否固定层级", currentIsFixedLayer);
            if (currentIsFixedLayer != newIsFixedLayer)
            {
                viewConfig.SetFixedLayer(newIsFixedLayer);
                EditorUtility.SetDirty(facade);
            }
        }

        private static void DrawUILayer(UIFacade facade, ViewConfig viewConfig)
        {
            ushort currentLayer = viewConfig.Layer;
            ushort newLayer = (ushort)SirenixEditorFields.IntField("UI层级", currentLayer);
            if (currentLayer != newLayer)
            {
                // 固定层级时才检查层级重复，非固定层级不参与冲突检查
                if (viewConfig.IsFixedLayer &&
                    UIFacadeInspectorViewConfigBehavior.CheckLayerDuplicate(facade, viewConfig, newLayer,
                        out string duplicateConfigID))
                {
                    if (EditorUtility.DisplayDialog("层级重复警告",
                            $"层级 {newLayer} 已被配置 '{duplicateConfigID}' 使用，是否继续？",
                            "继续", "取消"))
                    {
                        viewConfig.SetLayer(newLayer);
                    }
                }
                else
                {
                    viewConfig.SetLayer(newLayer);
                }
            }

            if (viewConfig.IsFixedLayer)
            {
                // 固定层级时显示冲突提示
                string conflictInfo = UIFacadeInspectorViewConfigBehavior.GetLayerConflictInfo(facade, viewConfig);
                if (!string.IsNullOrEmpty(conflictInfo))
                {
                    EditorGUILayout.HelpBox($"⚠ 层级冲突: 层级 {viewConfig.Layer} 已被以下配置使用: {conflictInfo}",
                        MessageType.Warning);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("非固定层级：当前层级值仅作参考，实际显示顺序由堆叠系统在运行时管理。", MessageType.Info);
            }
        }

        private static void DrawWindowMode(ViewConfig viewConfig)
        {
            bool currentIsWindow = viewConfig.IsWindow;
            bool newIsWindow = EditorGUILayout.Toggle("是否Window", currentIsWindow);
            if (currentIsWindow != newIsWindow)
            {
                viewConfig.SetWindow(newIsWindow);
            }
        }

        private static void DrawViewConfigTools(UIFacade facade, ViewConfig viewConfig)
        {
            SirenixEditorGUI.BeginBox("ViewConfig工具");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    // 同步ID按钮
                    GUI.backgroundColor = new Color(0.8f, 0.9f, 1f);
                    if (GUILayout.Button(new GUIContent("同步ID", "将UIFacade的ID同步到ViewConfig"), GUILayout.Height(25)))
                    {
                        SyncIDToViewConfig(facade, viewConfig);
                    }

                    // 保存ViewConfig到JSON按钮
                    GUI.backgroundColor = new Color(0.8f, 1f, 0.8f);
                    bool canSave = !string.IsNullOrEmpty(facade.m_ScriptName);
                    EditorGUI.BeginDisabledGroup(!canSave);
                    if (GUILayout.Button(
                            new GUIContent("保存ViewConfig", canSave ? "保存ViewConfig到JSON文件并生成读取类" : "脚本名称为空，无法保存"),
                            GUILayout.Height(25)))
                    {
                        SaveViewConfigToJson(facade, viewConfig);
                    }

                    EditorGUI.EndDisabledGroup();

                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();
            }
            SirenixEditorGUI.EndBox();
        }

        private static void SyncIDToViewConfig(UIFacade facade, ViewConfig viewConfig)
        {
            if (!string.IsNullOrEmpty(facade.m_ScriptName))
            {
                UIFacadeInspectorViewConfigBehavior.UpdateViewConfigID(facade, viewConfig);
                EditorUtility.SetDirty(facade);
                Debug.Log($"已同步配置ID: {facade.m_ScriptName}");
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "脚本名称为空，无法同步。请先在\"基本信息\"中填写模块名称和UI名称。", "确定");
            }
        }

        public static void SaveViewConfigToJson(UIFacade facade, ViewConfig viewConfig)
        {
            if (string.IsNullOrEmpty(facade.m_ScriptName))
            {
                EditorUtility.DisplayDialog("错误",
                    "脚本名称为空，无法保存ViewConfig！\n\n请先在\"基本信息\"中填写：\n- 模块名称\n- UI名称\n\n系统会自动生成脚本名称。", "确定");
                return;
            }

            // 确保ViewConfig的ID是最新的脚本名称
            UIFacadeInspectorViewConfigBehavior.UpdateViewConfigID(facade, viewConfig);

            // 更新AssetID
            UIFacadeInspectorViewConfigBehavior.UpdateViewConfigAssetID(facade, viewConfig);

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

            EditorUtility.DisplayDialog("成功", $"ViewConfig已保存并生成类型注册表", "确定");
            Debug.Log($"ViewConfig已保存: {facade.m_ScriptName}");
        }
    }
}
