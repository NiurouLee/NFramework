using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.IO;

using XHFramework;
namespace NFramework.ModuleSystem
{
    /// <summary>
    /// UIFacade工具栏绘制器
    /// </summary>
    public static class UIFacadeToolsDrawer
    {
        /// <summary>
        /// 绘制工具栏区域
        /// </summary>
        public static void DrawToolButtons(UIFacade facade, ref bool foldout, System.Action onDataChanged)
        {
            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();
            foldout = EditorGUILayout.Foldout(foldout, "工具栏", true);
            SirenixEditorGUI.EndBoxHeader();

            if (foldout)
            {
                SirenixEditorGUI.InfoMessageBox("使用以下工具来管理UI元素和配置");

                EditorGUILayout.Space(3);

                // 第一行工具按钮
                EditorGUILayout.BeginHorizontal();
                {
                    // 自动收集按钮
                    GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);
                    if (GUILayout.Button(new GUIContent("自动收集子对象", "自动收集GameObject下所有满足准入规则的组件"),
                            GUILayout.Height(22)))
                    {
                        UIFacadeInspectorUIElementBehavior.AutoCollectChildComponents(facade);
                        onDataChanged?.Invoke();
                    }

                    // 清空按钮
                    GUI.backgroundColor = new Color(1f, 0.7f, 0.7f);
                    if (GUILayout.Button(new GUIContent("清空列表", "清空所有UI元素"), GUILayout.Height(22)))
                    {
                        UIFacadeInspectorUIElementBehavior.ClearUIElements(facade);
                        onDataChanged?.Invoke();
                    }

                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(3);

                // 第二行工具按钮
                EditorGUILayout.BeginHorizontal();
                {
                    // 验证配置按钮
                    GUI.backgroundColor = new Color(0.9f, 0.9f, 0.7f);
                    if (GUILayout.Button(new GUIContent("验证配置", "验证当前配置是否正确"), GUILayout.Height(22)))
                    {
                        UIFacadeInspectorUIElementBehavior.ValidateConfiguration(facade);
                    }

                    // 生成脚本按钮
                    GUI.backgroundColor = new Color(0.7f, 0.9f, 1f);
                    if (GUILayout.Button(new GUIContent("生成脚本", "根据配置生成UI脚本"), GUILayout.Height(22)))
                    {
                        UIFacadeScriptGenerator.GenerateScript(facade);
                    }

                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(3);

                // 第三行工具按钮
                EditorGUILayout.BeginHorizontal();
                {
                    // 保存配置按钮
                    GUI.backgroundColor = new Color(0.8f, 0.8f, 1f);
                    if (GUILayout.Button(new GUIContent("保存配置", "保存当前配置到Prefab"), GUILayout.Height(22)))
                    {
                        SaveComponent(facade);
                    }

                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(3);

                // 第四行工具按钮
                EditorGUILayout.BeginHorizontal();
                {
                    // 打开脚本按钮
                    if (!string.IsNullOrEmpty(facade.m_ScriptName))
                    {
                        string logicRootPath = FrameworkConfigAssetEditor.GetConfig().UISystemConfig.LogicScriptGenerateRootPath;
                        string logicFilePath = Path.Combine(logicRootPath, facade.m_ModuleName ?? "", "UI", facade.m_ScriptName + ".cs");
                        bool logicFileExists = File.Exists(logicFilePath);

                        EditorGUI.BeginDisabledGroup(!logicFileExists);
                        GUI.backgroundColor = new Color(0.7f, 1f, 1f);
                        if (GUILayout.Button(new GUIContent("打开 Logic 脚本", "打开对应的逻辑脚本文件"), GUILayout.Height(22)))
                        {
                            Object scriptAsset = AssetDatabase.LoadAssetAtPath<Object>(logicFilePath);
                            if (scriptAsset != null)
                            {
                                AssetDatabase.OpenAsset(scriptAsset);
                            }
                            else
                            {
                                Debug.LogError($"找不到脚本文件: {logicFilePath}");
                            }
                        }

                        GUI.backgroundColor = Color.white;
                        EditorGUI.EndDisabledGroup();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            SirenixEditorGUI.EndBox();
        }

        private static void SaveComponent(UIFacade facade)
        {
            if (facade == null) return;

            // 保存ViewConfig
            ViewConfig viewConfig = UIConfigUtilsEditor.GetViewConfig(facade);
            UIFacadeViewConfigDrawer.SaveViewConfigToJson(facade, viewConfig);

            EditorUtility.SetDirty(facade);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("成功", "配置已保存！", "确定");
        }
    }
}
