using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// 在 Hierarchy 中为被 UIFacade 引用的物体绘制标记：
    /// - Toggle：物体自身或其子物体是否已被 UIFacade 引用；点击可自动加入（每次只加一个组件）/移出 UIFacade 的 UI 元素列表
    /// - 按钮：反向定位到 UIFacade 中的 UI 元素（选中 UIFacade 并展开对应元素）
    /// </summary>
    [InitializeOnLoad]
    public static class UIFacadeHierarchyMarker
    {
        private const string ExpandListPrefKey = "UIFacade_ExpandUIElements_{0}";

        static UIFacadeHierarchyMarker()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemGUI;
        }

        private static void OnHierarchyWindowItemGUI(int instanceID, Rect selectionRect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null)
            {
                return;
            }

            // 向上找 UIFacade：物体自身带 UIFacade 时跳过自己，找父级
            UIFacade facade = FindParentFacade(go);
            if (facade == null)
            {
                return;
            }

            bool isReferenced = IsReferencedByFacade(facade, go);

            // 该物体自身已录入的组件（每个组件在 Toggle 左侧显示一个缩写按钮，取消收录后自动消失）
            Component[] recordedComponents = GetRecordedComponentsOnSelf(facade, go);

            // 右对齐：定位按钮靠最右，Toggle 在其左侧，组件缩写按钮从 Toggle 向左排列
            float rightLimit = selectionRect.xMax - 4;
            Rect buttonRect = new Rect(rightLimit - 34, selectionRect.y + 1, 34, selectionRect.height - 2);
            Rect toggleRect = new Rect(buttonRect.x - 2 - 16, selectionRect.y + 1, 16, selectionRect.height - 2);

            var componentButtonRects = new Rect[recordedComponents.Length];
            float componentX = toggleRect.x - 2;
            for (int i = 0; i < recordedComponents.Length; i++)
            {
                componentX -= 20;
                componentButtonRects[i] = new Rect(componentX, selectionRect.y + 1, 20, selectionRect.height - 2);
                componentX -= 2;
            }

            Event currentEvent = Event.current;

            // 点击处理必须在绘制之前，避免被 GUI 控件或行选择事件干扰
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
            {
                for (int i = 0; i < componentButtonRects.Length; i++)
                {
                    if (componentButtonRects[i].Contains(currentEvent.mousePosition))
                    {
                        LocateElementByComponent(facade, recordedComponents[i]);
                        currentEvent.Use();
                        return;
                    }
                }

                if (toggleRect.Contains(currentEvent.mousePosition))
                {
                    if (isReferenced)
                    {
                        RemoveReferences(facade, go);
                    }
                    else
                    {
                        AddReferences(facade, go);
                    }

                    currentEvent.Use();
                    RepaintWindows();
                    return;
                }

                if (buttonRect.Contains(currentEvent.mousePosition))
                {
                    LocateElement(facade, go);
                    currentEvent.Use();
                    return;
                }
            }

            // 只在 Repaint 时绘制，控件不做交互，交互全部走上面的手动点击处理
            if (currentEvent.type == EventType.Repaint)
            {
                for (int i = 0; i < recordedComponents.Length; i++)
                {
                    string abbreviation = recordedComponents[i].GetType().Name.Substring(0, 1);
                    GUI.Button(componentButtonRects[i], new GUIContent(abbreviation,
                        $"已录入组件：{recordedComponents[i].GetType().Name}；点击定位到 Inspector 中的元素"));
                }

                GUI.Toggle(toggleRect, isReferenced,
                    new GUIContent("", "该物体或其子物体是否被 UIFacade 引用；点击加入（每次只加一个组件）/移除"));
                GUI.Button(buttonRect, new GUIContent("定位", "定位到 UIFacade 中的 UI 元素"));
            }
        }

        /// <summary>
        /// 查找该物体自身已录入 UIFacade 列表的全部组件（不含子物体的）
        /// </summary>
        private static Component[] GetRecordedComponentsOnSelf(UIFacade facade, GameObject go)
        {
            if (facade.m_UIElements == null)
            {
                return System.Array.Empty<Component>();
            }

            var result = new List<Component>();
            for (int i = 0; i < facade.m_UIElements.Count; i++)
            {
                var element = facade.m_UIElements[i];
                Component component = element != null ? element.Component as Component : null;
                if (component != null && component.gameObject == go)
                {
                    result.Add(component);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 向上查找最近的 UIFacade（严格不含自身；自身就是 UIFacade 时跳过，继续向上找）
        /// </summary>
        private static UIFacade FindParentFacade(GameObject go)
        {
            Transform parent = go != null ? go.transform.parent : null;
            while (parent != null)
            {
                UIFacade facade = parent.GetComponent<UIFacade>();
                if (facade != null)
                {
                    return facade;
                }

                parent = parent.parent;
            }

            return null;
        }

        private static void RepaintWindows()
        {
            EditorApplication.RepaintHierarchyWindow();
            EditorWindow.focusedWindow?.Repaint();
        }

        private static bool IsReferencedByFacade(UIFacade facade, GameObject go)
        {
            if (facade.m_UIElements == null)
            {
                return false;
            }

            for (int i = 0; i < facade.m_UIElements.Count; i++)
            {
                var element = facade.m_UIElements[i];
                Component component = element != null ? element.Component as Component : null;
                if (component != null && IsSelfOrDescendant(component.gameObject, go))
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddReferences(UIFacade facade, GameObject go)
        {
            // 每次只加入一个组件：优先取该物体的 RectTransform，否则取第一个满足准入规则的组件
            Component component = GetSingleAddableComponent(go);
            if (component == null)
            {
                EditorUtility.DisplayDialog("提示",
                    $"物体 '{go.name}' 上没有可加入的组件。", "确定");
                return;
            }

            int added = UIFacadeInspectorUIElementBehavior.AddUIComponentCandidates(facade, new[] { component });
            if (added == 0)
            {
                EditorUtility.DisplayDialog("提示",
                    $"物体 '{go.name}' 的组件已在列表中。", "确定");
            }
        }

        private static Component GetSingleAddableComponent(GameObject go)
        {
            if (go == null)
            {
                return null;
            }

            // 优先只加 RectTransform（UI 物体上都有）
            RectTransform rectTransform = go.GetComponent<RectTransform>();
            if (rectTransform != null && UIFacadeElementRules.CanAddToElementList(rectTransform))
            {
                return rectTransform;
            }

            // 否则取该物体自身第一个满足准入规则的组件
            Component[] candidates = UIFacadeInspectorUIElementBehavior.GetUIComponentCandidates(go, false);
            return candidates.Length > 0 ? candidates[0] : null;
        }

        private static void RemoveReferences(UIFacade facade, GameObject go)
        {
            if (facade.m_UIElements == null)
            {
                return;
            }

            facade.m_UIElements.RemoveAll(element =>
            {
                Component component = element != null ? element.Component as Component : null;
                return component != null && IsSelfOrDescendant(component.gameObject, go);
            });

            EditorUtility.SetDirty(facade);
        }

        private static bool IsSelfOrDescendant(GameObject target, GameObject root)
        {
            Transform transform = target != null ? target.transform : null;
            while (transform != null)
            {
                if (transform.gameObject == root)
                {
                    return true;
                }

                transform = transform.parent;
            }

            return false;
        }

        private static void LocateElement(UIFacade facade, GameObject go)
        {
            // 定位该物体（或其子树）对应的第一个已录入元素
            LocateElementByComponent(facade, FindFirstRecordedComponent(facade, go));
        }

        private static void LocateElementByComponent(UIFacade facade, Component targetComponent)
        {
            int componentId = targetComponent != null ? targetComponent.GetInstanceID() : 0;

            // 请求 Inspector 展开 UI 元素列表，并高亮/聚焦对应的元素
            EditorPrefs.SetBool(string.Format(ExpandListPrefKey, facade.GetInstanceID()), true);
            if (componentId != 0)
            {
                EditorPrefs.SetInt($"UIFacade_LocateElement_{facade.GetInstanceID()}", componentId);
            }

            // 在 Hierarchy 点击事件里直接改 Selection 会被窗口后续处理还原，放到 delayCall 中执行；
            // 同时强制 Inspector 重绘，否则已选中同一 Facade 时高亮不会刷新
            GameObject facadeGameObject = facade.gameObject;
            EditorApplication.delayCall += () =>
            {
                Selection.activeGameObject = facadeGameObject;
                EditorGUIUtility.PingObject(facadeGameObject);
                RepaintInspectorWindows();
                EditorWindow.focusedWindow?.Repaint();
            };
        }

        /// <summary>
        /// 找到该物体（或其子树）对应的第一个已录入组件
        /// </summary>
        private static Component FindFirstRecordedComponent(UIFacade facade, GameObject go)
        {
            if (facade.m_UIElements != null)
            {
                for (int i = 0; i < facade.m_UIElements.Count; i++)
                {
                    var element = facade.m_UIElements[i];
                    Component component = element != null ? element.Component as Component : null;
                    if (component != null && IsSelfOrDescendant(component.gameObject, go))
                    {
                        return component;
                    }
                }
            }

            return null;
        }

        private static void RepaintInspectorWindows()
        {
            System.Type inspectorType = System.Type.GetType("UnityEditor.InspectorWindow, UnityEditor");
            Object[] windows = inspectorType != null
                ? Resources.FindObjectsOfTypeAll(inspectorType)
                : Resources.FindObjectsOfTypeAll<EditorWindow>();

            for (int i = 0; i < windows.Length; i++)
            {
                if (windows[i] is EditorWindow editorWindow)
                {
                    editorWindow.Repaint();
                }
            }
        }

    }
}
