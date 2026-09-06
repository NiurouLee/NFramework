using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    /// <summary>
    /// [UIROOT] 上 UISystemWindowRequestDebugger 的自定义 Inspector：
    /// 在编辑器 Play Mode 下用两个列表展示：
    /// 1. Requests：当前 UISystem 中所有 WindowRequest（名字/状态/Key/Cache 对象）
    /// 2. Pool：窗口 LRU 池中缓存的窗口。
    /// </summary>
    [CustomEditor(typeof(UISystemWindowRequestDebugger))]
    public class UISystemWindowRequestDebuggerEditor : Editor
    {
        private const double RefreshIntervalSeconds = 0.2;

        private double m_NextRepaintTime;
        private bool m_AutoRefresh = true;
        private bool m_RequestsListExpanded = true;
        private bool m_PoolListExpanded = true;

        private GUIStyle m_TitleStyle;
        private GUIStyle m_ValueStyle;
        private GUIStyle m_FieldLabelStyle;

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
            m_NextRepaintTime = 0;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (this.target == null || !EditorApplication.isPlaying || !m_AutoRefresh)
            {
                return;
            }

            if (EditorApplication.timeSinceStartup < m_NextRepaintTime)
            {
                return;
            }

            m_NextRepaintTime = EditorApplication.timeSinceStartup + RefreshIntervalSeconds;
            this.Repaint();
        }

        public override void OnInspectorGUI()
        {
            EnsureStyles();

            var debugger = this.target as UISystemWindowRequestDebugger;
            if (debugger == null)
            {
                return;
            }

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox(
                    "该监视组件只在编辑器 Play Mode 下由 UISystem 自动附加到 [UIROOT]。\n" +
                    "进入 Play Mode 后选中 [UIROOT] 节点即可查看所有 WindowRequest 状态。",
                    MessageType.Info);
                return;
            }

            var uiSystem = debugger.UISystem;
            if (uiSystem == null)
            {
                EditorGUILayout.HelpBox("尚未绑定到 UISystem，正在等待 UISystem.AwakeRoot 完成绑定…", MessageType.Warning);
                return;
            }

            DrawState(uiSystem);
        }

        private void EnsureStyles()
        {
            if (m_TitleStyle != null)
            {
                return;
            }

            m_TitleStyle = new GUIStyle(EditorStyles.boldLabel);
            m_ValueStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
            };
            m_FieldLabelStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
            };
        }

        private void DrawState(UISystem inUISystem)
        {
            var requests = inUISystem.DebugGetWindowRequests();
            requests.RemoveAll(request => request == null);
            requests.Sort(CompareRequest);

            var poolEntries = inUISystem.DebugGetWindowPoolEntries();
            poolEntries.RemoveAll(entry => entry.Key == null || entry.Value == null);

            DrawToolbar(requests.Count, poolEntries.Count, inUISystem.DebugWindowPoolEnabled);

            DrawRequestsList(requests, inUISystem);
            EditorGUILayout.Space(6);
            DrawPoolList(poolEntries, inUISystem.DebugWindowPoolEnabled);
        }

        private void DrawToolbar(int inRequestCount, int inPoolCount, bool inPoolEnabled)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("UISystem WindowRequest", m_TitleStyle);
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField($"Requests {inRequestCount}   Pool {inPoolCount}", m_TitleStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            m_AutoRefresh = EditorGUILayout.ToggleLeft("自动刷新", m_AutoRefresh, GUILayout.Width(92));
            GUILayout.Space(6);

            if (GUILayout.Button("展开列表"))
            {
                m_RequestsListExpanded = true;
                m_PoolListExpanded = true;
            }

            if (GUILayout.Button("收起列表"))
            {
                m_RequestsListExpanded = false;
                m_PoolListExpanded = false;
            }

            EditorGUILayout.EndHorizontal();

            if (!inPoolEnabled)
            {
                EditorGUILayout.HelpBox("窗口池未启用（UISystemConfig.EnableWindowPool = false）。", MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawRequestsList(List<WindowRequest> inRequests, UISystem inUISystem)
        {
            Color oldColor = GUI.color;
            GUI.color = EditorGUIUtility.isProSkin
                ? new Color(0.4f, 0.9f, 1f)
                : new Color(0f, 0.42f, 0.78f);
            m_RequestsListExpanded = EditorGUILayout.Foldout(
                m_RequestsListExpanded,
                $"  Requests  ({inRequests.Count})",
                true,
                EditorStyles.foldoutHeader);
            GUI.color = oldColor;

            if (!m_RequestsListExpanded)
            {
                return;
            }

            if (inRequests.Count == 0)
            {
                EditorGUILayout.HelpBox("当前没有正在处理/打开的 WindowRequest。", MessageType.Info);
                return;
            }

            EditorGUI.indentLevel++;
            for (int i = 0; i < inRequests.Count; i++)
            {
                DrawRequestCard(inRequests[i], i + 1, inUISystem);
                EditorGUILayout.Space(2);
            }

            EditorGUI.indentLevel--;
        }

        private void DrawPoolList(List<KeyValuePair<string, WindowPoolEntry>> inPoolEntries, bool inPoolEnabled)
        {
            Color oldColor = GUI.color;
            GUI.color = EditorGUIUtility.isProSkin
                ? new Color(0.55f, 0.9f, 1f)
                : new Color(0f, 0.52f, 0.66f);
            m_PoolListExpanded = EditorGUILayout.Foldout(
                m_PoolListExpanded,
                $"  Pool  ({inPoolEntries.Count})",
                true,
                EditorStyles.foldoutHeader);
            GUI.color = oldColor;

            if (!m_PoolListExpanded)
            {
                return;
            }

            if (!inPoolEnabled)
            {
                return;
            }

            if (inPoolEntries.Count == 0)
            {
                EditorGUILayout.HelpBox("窗口池当前为空。", MessageType.Info);
                return;
            }

            EditorGUI.indentLevel++;
            foreach (var pair in inPoolEntries)
            {
                DrawPoolCard(pair.Key, pair.Value.Window);
                EditorGUILayout.Space(2);
            }

            EditorGUI.indentLevel--;
        }

        private static int CompareRequest(WindowRequest inLeft, WindowRequest inRight)
        {
            int layerCompare = (inLeft.Config?.Layer ?? 0).CompareTo(inRight.Config?.Layer ?? 0);
            if (layerCompare != 0)
            {
                return layerCompare;
            }

            int orderCompare = inLeft.CacheOrderObj.CompareTo(inRight.CacheOrderObj);
            if (orderCompare != 0)
            {
                return orderCompare;
            }

            return string.CompareOrdinal(inLeft.Name ?? string.Empty, inRight.Name ?? string.Empty);
        }

        private void DrawRequestCard(WindowRequest inRequest, int inIndex, UISystem inUISystem)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // 标题行：名字 + 状态
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{inIndex.ToString("00")}  {GetDisplayName(inRequest.Name)}", m_TitleStyle);
            GUILayout.FlexibleSpace();

            if (inUISystem.DebugIsRequestCanceled(inRequest))
            {
                Color oldColor = GUI.color;
                GUI.color = Color.red;
                GUILayout.Label("已取消", m_TitleStyle);
                GUI.color = oldColor;
            }

            Color stageOldColor = GUI.color;
            GUI.color = GetStageColor(inRequest.Stage);
            GUILayout.Label(inRequest.Stage.ToString(), m_TitleStyle);
            GUI.color = stageOldColor;
            EditorGUILayout.EndHorizontal();

            DrawField("Key", FormatKeyString(inRequest.keyObj));
            DrawField("RequestKey", FormatRequestKey(inRequest));
            DrawField("Window", FormatWindow(inRequest.CacheWindowObj));
            DrawField("Facade", FormatFacade(inRequest.CacheFacadeObj));
            DrawField("ViewData", FormatObject(inRequest.CacheViewDataObj));
            DrawField("Provider", FormatObject(inRequest.CacheProviderObj));
            DrawField("Layer", FormatLayer(inRequest.Config));
            DrawField("Order", inRequest.CacheOrderObj.ToString());

            EditorGUILayout.EndVertical();
        }

        private void DrawPoolCard(string inRequestKey, Window inWindow)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(inWindow == null ? "(null)" : inWindow.GetType().Name, m_TitleStyle);
            GUILayout.FlexibleSpace();

            if (inWindow != null && inWindow.Facade != null)
            {
                Color oldColor = GUI.color;
                GUI.color = new Color(0.5f, 0.85f, 1f);
                GUILayout.Label("Pooled", m_TitleStyle);
                GUI.color = oldColor;
            }

            EditorGUILayout.EndHorizontal();

            DrawField("RequestKey", FormatKeyString(inRequestKey));
            DrawField("Window Key", inWindow == null ? "-" : FormatKeyString(inWindow.Key));
            DrawField("Facade", inWindow == null ? "null" : FormatFacade(inWindow.Facade));

            EditorGUILayout.EndVertical();
        }

        private static string FormatWindow(Window inWindow)
        {
            if (inWindow == null)
            {
                return "null";
            }

            string key = string.IsNullOrEmpty(inWindow.Key) ? "" : $"  key={FormatKeyString(inWindow.Key)}";
            return $"{inWindow.GetType().Name}{key}";
        }

        private static string FormatFacade(UIFacade inFacade)
        {
            if (inFacade == null)
            {
                return "null";
            }

            return $"{inFacade.name}  [{inFacade.GetType().Name}]";
        }

        private static string FormatObject(object inObject)
        {
            if (inObject == null)
            {
                return "null";
            }

            return inObject.GetType().Name;
        }

        private void DrawField(string inLabel, string inValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(inLabel, m_FieldLabelStyle, GUILayout.Width(100));
            EditorGUILayout.SelectableLabel(inValue ?? "-", m_ValueStyle);
            EditorGUILayout.EndHorizontal();
        }

        private static string FormatRequestKey(WindowRequest inRequest)
        {
            if (string.IsNullOrEmpty(inRequest.keyObj))
            {
                return inRequest.Name ?? string.Empty;
            }

            return FormatKeyString(inRequest.RequestKey);
        }

        private static string FormatKeyString(string inKey)
        {
            if (string.IsNullOrEmpty(inKey))
            {
                return "(默认)";
            }

            // RequestKey 内部用不可见分隔符连接窗口ID与业务 key，转成可见分隔符方便阅读
            return inKey.Replace('\u0001', '|');
        }

        private static string FormatLayer(ViewConfig inConfig)
        {
            if (inConfig == null)
            {
                return "-";
            }

            ushort layer = inConfig.Layer;
            if (layer == 0)
            {
                return "未设置(0)，按 Basic 处理";
            }

            return $"{layer}({(UILayer)layer})";
        }

        private static string GetDisplayName(string inName)
        {
            return string.IsNullOrEmpty(inName) ? "(未命名)" : inName;
        }

        private static Color GetStageColor(WindowRequestStage inStage)
        {
            bool darkSkin = EditorGUIUtility.isProSkin;
            switch (inStage)
            {
                case WindowRequestStage.Construct:
                case WindowRequestStage.Cache:
                    return darkSkin ? Color.cyan : new Color(0f, 0.4f, 0.72f);

                case WindowRequestStage.FacadeLoading:
                case WindowRequestStage.FacadeLoaded:
                case WindowRequestStage.Layer:
                case WindowRequestStage.WindowAwake:
                    return darkSkin ? Color.yellow : new Color(0.78f, 0.48f, 0f);

                case WindowRequestStage.WindowOpen:
                case WindowRequestStage.WindowOpenAnim:
                case WindowRequestStage.WindowOpened:
                    return darkSkin ? Color.green : new Color(0f, 0.52f, 0.2f);

                case WindowRequestStage.WindowClose:
                case WindowRequestStage.WindowCloseAnim:
                case WindowRequestStage.WindowClosed:
                case WindowRequestStage.GameObjectUnloading:
                    return darkSkin ? new Color(1f, 0.5f, 0.1f) : new Color(0.82f, 0.38f, 0f);

                default:
                    return darkSkin ? Color.gray : new Color(0.35f, 0.35f, 0.35f);
            }
        }
    }
}
