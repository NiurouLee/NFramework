using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// UIFacade UI元素列表绘制器
    /// </summary>
    public static class UIFacadeUIElementsDrawer
    {
        // 搜索筛选文本
        private static string m_SearchFilter = "";

        // 从 Hierarchy 定位：目标组件实例 ID + 高亮持续窗口（约 2 秒）
        private static int m_LocateComponentId;
        private static float m_LocateHighlightUntil;
        private static readonly Color m_LocateHighlight = new Color(1f, 0.84f, 0.2f, 0.22f);
        private static readonly Color m_LocateAccent = new Color(1f, 0.7f, 0f, 0.9f);

        private static readonly GUIStyle m_DropAreaStyle = new GUIStyle("Box")
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12,
        };

        /// <summary>
        /// 拖放区：将 GameObject 拖入后自动把其上的 UI 组件加入元素列表
        /// </summary>
        public static void DrawGameObjectDropArea(UIFacade facade, System.Action onDataChanged)
        {
            if (facade == null) return;

            Event currentEvent = Event.current;
            Rect dropRect = GUILayoutUtility.GetRect(0f, 36f, GUILayout.ExpandWidth(true));
            bool hasGameObject = DragAndDrop.objectReferences != null &&
                                 System.Array.Exists(DragAndDrop.objectReferences, o => o is GameObject);
            bool hover = hasGameObject && dropRect.Contains(currentEvent.mousePosition);

            GUI.color = hover ? new Color(0.4f, 0.8f, 1f, 0.35f) : Color.white;
            GUI.Box(dropRect, hover ? "松开鼠标，添加 UI 组件" : "拖入 GameObject，自动添加其 UI 组件到元素列表", m_DropAreaStyle);
            GUI.color = Color.white;

            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                    if (hasGameObject && dropRect.Contains(currentEvent.mousePosition))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        currentEvent.Use();
                    }
                    break;

                case EventType.DragPerform:
                    if (hasGameObject && dropRect.Contains(currentEvent.mousePosition))
                    {
                        DragAndDrop.AcceptDrag();
                        int addedCount = 0;
                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is GameObject draggedGameObject)
                            {
                                var components = UIFacadeInspectorUIElementBehavior.GetUIComponentCandidates(draggedGameObject);
                                addedCount += UIFacadeInspectorUIElementBehavior.AddUIComponentCandidates(facade, components);
                            }
                        }

                        if (addedCount > 0)
                        {
                            onDataChanged?.Invoke();
                        }

                        currentEvent.Use();
                    }
                    break;
            }
        }

        /// <summary>
        /// 绘制UI元素列表区域
        /// </summary>
        public static void DrawUIElementsList(UIFacade facade, ref bool foldout, System.Action onDataChanged)
        {
            // 支持从 Hierarchy 反向定位：一次性强制展开列表
            string expandPrefKey = $"UIFacade_ExpandUIElements_{facade.GetInstanceID()}";
            if (EditorPrefs.GetBool(expandPrefKey, false))
            {
                foldout = true;
                EditorPrefs.DeleteKey(expandPrefKey);
            }

            // 消费 Hierarchy 的定位请求，并让高亮持续一小段时间
            string locateKey = $"UIFacade_LocateElement_{facade.GetInstanceID()}";
            int pendingLocateId = EditorPrefs.GetInt(locateKey, 0);
            if (pendingLocateId != 0)
            {
                m_LocateComponentId = pendingLocateId;
                m_LocateHighlightUntil = (float)EditorApplication.timeSinceStartup + 2f;
                EditorPrefs.DeleteKey(locateKey);
            }

            int locateIndex = m_LocateComponentId != 0 &&
                              (float)EditorApplication.timeSinceStartup < m_LocateHighlightUntil
                ? FindElementIndexByComponentId(facade, m_LocateComponentId)
                : -1;

            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();
            foldout = EditorGUILayout.Foldout(foldout, "UI元素列表", true);
            SirenixEditorGUI.EndBoxHeader();
            
            if (foldout)
            {
                if (facade.m_UIElements == null)
                {
                    facade.m_UIElements = new List<UIFacade.UIElement>();
                }

                // 列表标题和添加按钮
                DrawToolbar(facade, onDataChanged);

                EditorGUILayout.Space(3);

                // 搜索筛选框
                DrawSearchFilter();

                EditorGUILayout.Space(3);

                // 如果列表为空，显示提示
                if (facade.m_UIElements.Count == 0)
                {
                    SirenixEditorGUI.InfoMessageBox("暂无UI元素，点击上方的 + 按钮添加新元素，或使用下方的\"自动收集子对象\"功能。");
                }
                else
                {
                    // 绘制元素列表（带筛选）
                    int visibleCount = 0;
                    for (int i = 0; i < facade.m_UIElements.Count; i++)
                    {
                        // 检查是否匹配搜索条件
                        if (UIFacadeInspectorUIElementsListBehavior.MatchSearchFilter(facade.m_UIElements[i], m_SearchFilter))
                        {
                            // 绘制元素
                            DrawUIElement(facade, i, onDataChanged, i == locateIndex);
                            visibleCount++;
                        }
                    }

                    // 聚焦目标元素，让 Inspector 滚动到可见位置
                    if (locateIndex >= 0)
                    {
                        EditorGUI.FocusTextInControl(GetElementNameControlId(facade, locateIndex));
                    }

                    // 如果有搜索条件但没有匹配结果
                    if (!string.IsNullOrEmpty(m_SearchFilter) && visibleCount == 0)
                    {
                        SirenixEditorGUI.InfoMessageBox($"没有找到匹配 \"{m_SearchFilter}\" 的元素");
                    }
                }
            }

            SirenixEditorGUI.EndBox();
        }

        private static int FindElementIndexByComponentId(UIFacade facade, int componentId)
        {
            if (componentId == 0 || facade.m_UIElements == null)
            {
                return -1;
            }

            for (int i = 0; i < facade.m_UIElements.Count; i++)
            {
                var element = facade.m_UIElements[i];
                if (element != null && element.Component != null && element.Component.GetInstanceID() == componentId)
                {
                    return i;
                }
            }

            return -1;
        }

        private static string GetElementNameControlId(UIFacade facade, int index)
        {
            return $"UIElementName_{facade.GetInstanceID()}_{index}";
        }

        private static void DrawToolbar(UIFacade facade, System.Action onDataChanged)
        {
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.Label($"元素数量: {facade.m_UIElements.Count}", SirenixGUIStyles.LeftAlignedGreyMiniLabel);
                GUILayout.FlexibleSpace();

                // 添加按钮
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button(new GUIContent("+", "添加新UI元素"),
                        GUILayout.Width(25), GUILayout.Height(18)))
                {
                    UIFacadeInspectorUIElementsListBehavior.AddNewUIElement(facade);
                    onDataChanged?.Invoke();
                }

                GUI.backgroundColor = Color.white;

                // 刷新按钮
                if (GUILayout.Button(new GUIContent("↻", "刷新并清理无效元素"),
                        GUILayout.Width(25), GUILayout.Height(18)))
                {
                    UIFacadeInspectorUIElementsListBehavior.RefreshUIElements(facade);
                    onDataChanged?.Invoke();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        /// <summary>
        /// 绘制搜索筛选框
        /// </summary>
        private static void DrawSearchFilter()
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("搜索:", GUILayout.Width(40));

                // 搜索输入框
                EditorGUI.BeginChangeCheck();
                m_SearchFilter = EditorGUILayout.TextField(m_SearchFilter, EditorStyles.toolbarSearchField);

                // 清除按钮
                if (!string.IsNullOrEmpty(m_SearchFilter))
                {
                    if (GUILayout.Button("×", GUILayout.Width(18), GUILayout.Height(18)))
                    {
                        m_SearchFilter = "";
                        GUI.FocusControl(null);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            // 显示搜索提示
            if (string.IsNullOrEmpty(m_SearchFilter))
            {
                EditorGUILayout.LabelField("", "支持搜索元素名称和组件类型", SirenixGUIStyles.RightAlignedGreyMiniLabel);
            }
        }

        /// <summary>
        /// 检查元素是否匹配搜索条件（模糊搜索）
        /// </summary>
        private static bool MatchSearchFilter(UIFacade.UIElement element)
        {
            // 如果没有搜索条件，显示所有元素
            if (string.IsNullOrEmpty(m_SearchFilter))
                return true;

            if (element == null)
                return false;

            string filter = m_SearchFilter.ToLower();

            // 搜索元素名称（模糊匹配）
            if (!string.IsNullOrEmpty(element.Name))
            {
                if (element.Name.ToLower().Contains(filter))
                    return true;
            }

            // 搜索组件类型名称（模糊匹配）
            if (element.Component != null)
            {
                string typeName = element.Component.GetType().Name.ToLower();
                if (typeName.Contains(filter))
                    return true;
            }

            return false;
        }

        private static void DrawUIElement(UIFacade facade, int index, System.Action onDataChanged, bool isTarget = false)
        {
            if (facade.m_UIElements == null || index < 0 || index >= facade.m_UIElements.Count)
                return;

            var element = facade.m_UIElements[index];
            if (element == null)
            {
                element = new UIFacade.UIElement();
                facade.m_UIElements[index] = element;
            }

            bool isExpanded = EditorPrefs.GetBool($"UIElement_{facade.GetInstanceID()}_{index}", true);

            SirenixEditorGUI.BeginBox();
            {
                // 第一行：折叠开关 + 索引 + 名称（内联编辑）+ 组件 + 删除按钮，合并为一行
                SirenixEditorGUI.BeginHorizontalToolbar();
                {
                    isExpanded = EditorGUILayout.Foldout(isExpanded, "", true);
                    EditorPrefs.SetBool($"UIElement_{facade.GetInstanceID()}_{index}", isExpanded);

                    GUILayout.Label($"{index}", GUILayout.Width(22));

                    // 名称字段（内联编辑）
                    EditorGUI.BeginChangeCheck();
                    GUI.SetNextControlName(GetElementNameControlId(facade, index));
                    string newName = EditorGUILayout.TextField(
                        element.Name ?? "", GUILayout.MinWidth(60), GUILayout.ExpandWidth(true));
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (UIFacadeInspectorUIElementsListBehavior.UpdateUIElementName(facade, index, newName))
                        {
                            onDataChanged?.Invoke();
                        }
                    }

                    // 组件下拉选择：已有组件时只列出该组件所在物体身上的组件；
                    // 空元素（还没有组件）才列出整个 Facade 下的候选，方便首次选择
                    Component[] candidates;
                    if (element.Component is Component currentComponent && currentComponent != null)
                    {
                        candidates = UIFacadeInspectorUIElementBehavior.GetUIComponentCandidates(currentComponent.gameObject, false);
                    }
                    else
                    {
                        candidates = UIFacadeInspectorUIElementBehavior.GetUIComponentCandidates(facade);
                    }
                    var itemList = new List<Component>(candidates);
                    var displayNames = new List<string> { "(无)" };
                    int currentIndex = 0;
                    for (int i = 0; i < itemList.Count; i++)
                    {
                        displayNames.Add($"{itemList[i].gameObject.name} ({itemList[i].GetType().Name})");
                        if (ReferenceEquals(itemList[i], element.Component))
                        {
                            currentIndex = i + 1;
                        }
                    }

                    // 当前引用的组件已不在候选中（可能被删除），保留一项避免误显示为“无”
                    if (currentIndex == 0 && element.Component != null)
                    {
                        itemList.Insert(0, element.Component as Component);
                        displayNames.Insert(1, $"⚠ {element.Component.name} ({element.Component.GetType().Name})");
                        currentIndex = 1;
                    }

                    EditorGUI.BeginChangeCheck();
                    int selectedIndex = EditorGUILayout.Popup(
                        currentIndex, displayNames.ToArray(),
                        GUILayout.MinWidth(120), GUILayout.MaxWidth(200));
                    if (EditorGUI.EndChangeCheck() && selectedIndex != currentIndex)
                    {
                        Component selected = selectedIndex == 0 ? null : itemList[selectedIndex - 1];
                        if (UIFacadeInspectorUIElementsListBehavior.UpdateUIElementComponent(facade, element, selected))
                        {
                            onDataChanged?.Invoke();
                        }
                    }

                    // 复制按钮
                    if (GUILayout.Button(new GUIContent("复制", "在下方复制此元素"), GUILayout.Width(32), GUILayout.Height(16)))
                    {
                        UIFacadeInspectorUIElementsListBehavior.DuplicateUIElement(facade, index);
                        onDataChanged?.Invoke();
                    }

                    // 删除按钮
                    GUI.color = Color.red;
                    if (GUILayout.Button("×", GUILayout.Width(18), GUILayout.Height(16)))
                    {
                        if (EditorUtility.DisplayDialog("确认删除", $"确定要删除元素 '{element.Name}' 吗？", "确定", "取消"))
                        {
                            UIFacadeInspectorUIElementsListBehavior.RemoveUIElement(facade, index);
                            onDataChanged?.Invoke();
                            return;
                        }
                    }

                    GUI.color = Color.white;
                }
                SirenixEditorGUI.EndHorizontalToolbar();

                // 展开时显示描述（单行，更紧凑）
                if (isExpanded)
                {
                    DrawElementDesc(facade, element, onDataChanged);
                }
            }
            SirenixEditorGUI.EndBox();

            // 高亮定位目标元素
            if (isTarget && Event.current.type == EventType.Repaint)
            {
                Rect elementRect = GUILayoutUtility.GetLastRect();
                EditorGUI.DrawRect(elementRect, m_LocateHighlight);
                EditorGUI.DrawRect(new Rect(elementRect.x, elementRect.y, 3, elementRect.height), m_LocateAccent);
            }
        }

        private static void DrawElementDesc(UIFacade facade, UIFacade.UIElement element, System.Action onDataChanged)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(15); // 缩进
                GUILayout.Label("描述:", GUILayout.Width(35));
                EditorGUI.BeginChangeCheck();
                string newDesc = EditorGUILayout.TextField(element.Desc ?? "");
                if (EditorGUI.EndChangeCheck())
                {
                    UIFacadeInspectorUIElementsListBehavior.UpdateUIElementDesc(facade, element, newDesc);
                    onDataChanged?.Invoke();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
