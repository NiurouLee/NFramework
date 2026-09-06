using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using GameObject = UnityEngine.GameObject;
using Vector3 = UnityEngine.Vector3;

namespace NFramework.ModuleSystem
{
    public partial class UISystem : FrameworkSystemModuleBase
    {
        private GameObject uiRoot;

        public Camera UICamera { get; private set; }
        public Canvas UICanvas { get; private set; }
        public Transform UICanvasTrf { get; private set; }
        public UnityEngine.EventSystems.EventSystem EventSystem { get; private set; }

        private CanvasScaler scaler;

        /// <summary>每个 UILayer 一个独立 Stack 层级：Layer 枚举值 -> 层级服务</summary>
        private SortedDictionary<ushort, UIStackLayerServices> m_LayerStacks;


        private WindowPoolServices m_Pool;
        private bool m_EnableWindowPool;

        public override void Awake()
        {
            GetSystem<LoggerSystem>().Error?.Print("UIM Awake");
            base.Awake();
            var go = UnityEngine.Resources.Load<GameObject>("UIROOT");
            var root = UnityEngine.Object.Instantiate(go);
            this.AwakeRoot(root);
            this.AwakeConfigServices();
            this.AwakeLayerServices();
            this.AwakePoolServices();
        }

        public void AwakeRoot(GameObject inRoot)
        {
            uiRoot = inRoot;
            uiRoot.transform.localPosition = new Vector3(0, 1000, 0);
            uiRoot.name = "[UIROOT]";
            UnityEngine.GameObject.DontDestroyOnLoad(uiRoot);
#if UNITY_EDITOR
            // 仅在编辑器 Play Mode 下给 [UIROOT] 挂 WindowRequest 状态监视组件；
            // 方便在 Inspector 直接观察当前 UISystem 内所有请求的状态，不参与正式构建。
            if (Application.isPlaying)
            {
                var debugger = uiRoot.GetComponent<UISystemWindowRequestDebugger>();
                if (debugger == null)
                {
                    debugger = uiRoot.AddComponent<UISystemWindowRequestDebugger>();
                }

                debugger.Bind(this);
            }
#endif
            UICamera = uiRoot.GetComponentInChildren<Camera>();
            var mainCamera = UnityEngine.Camera.main;
            mainCamera.GetComponent<UniversalAdditionalCameraData>().cameraStack.Add(UICamera);
            UICanvasTrf = uiRoot.transform.Find("Canvas");
            UICanvas = UICanvasTrf.GetComponent<Canvas>();
            EventSystem = uiRoot.GetComponentInChildren<UnityEngine.EventSystems.EventSystem>();
            scaler = this.UICanvas.GetOrAddComponent<CanvasScaler>();
        }

        public void AwakeLayerServices()
        {
            m_LayerStacks = new SortedDictionary<ushort, UIStackLayerServices>();

            foreach (UILayer layer in System.Enum.GetValues(typeof(UILayer)))
            {
                var layerGo = new GameObject($"[{layer}]");
                var layerRectTransform = layerGo.AddComponent<RectTransform>();
                layerRectTransform.SetParent(this.UICanvasTrf);
                layerRectTransform.localPosition = new Vector3(0, 0, 0);
                layerRectTransform.localScale = new Vector3(1, 1, 1);
                layerRectTransform.localRotation = Quaternion.identity;
                layerRectTransform.anchorMin = new Vector2(0, 0);
                layerRectTransform.anchorMax = new Vector2(1, 1);
                layerRectTransform.pivot = new Vector2(0.5f, 0.5f);
                layerRectTransform.anchoredPosition = new Vector2(0, 0);
                layerRectTransform.sizeDelta = new Vector2(0, 0);

                var stack = new UIStackLayerServices(this, layerGo, (ushort)layer);
                m_LayerStacks.Add((ushort)layer, stack);
            }
        }

        private void AwakePoolServices()
        {
            var uiConfig = NFROOT.Instance?.Config?.UISystemConfig;
            m_EnableWindowPool = uiConfig != null && uiConfig.EnableWindowPool;

            if (!m_EnableWindowPool)
            {
                this.m_Pool = null;
                return;
            }

            int poolSize = Mathf.Clamp(uiConfig.WindowPoolSize, 1, 256);
            this.m_Pool = new WindowPoolServices(this, poolSize);
        }

        /// <summary>窗口池当前是否启用</summary>
        private bool WindowPoolEnabled => this.m_EnableWindowPool && this.m_Pool != null;

        /// <summary>
        /// 根据 ViewConfig.Layer 获取对应 Stack。
        /// Layer 为 0 表示配置里还没选层级，默认进 Basic，避免旧配置直接失效。
        /// </summary>
        private bool TryGetLayerStack(ushort inLayer, out UIStackLayerServices outLayerStack)
        {
            ushort layer = inLayer == 0 ? (ushort)UILayer.Basic : inLayer;
            if (m_LayerStacks != null && m_LayerStacks.TryGetValue(layer, out outLayerStack))
            {
                return true;
            }

            outLayerStack = null;
            return false;
        }

        public void __WindowSetUpLayer(ViewConfig inViewConfig, Window inWindow, UIFacade inFacade, int inOrder)
        {
            if (inViewConfig == null)
            {
                this.GetSystem<LoggerSystem>()?.ErrStack("__WindowSetUpLayer inViewConfig is null");
                return;
            }

            if (this.TryGetLayerStack(inViewConfig.Layer, out var layerStack))
            {
                layerStack.PushWindow(inWindow, inViewConfig, inFacade, inOrder);
            }
            else
            {
                this.GetSystem<LoggerSystem>()
                    ?.ErrStack(
                        $"__WindowSetUpLayer can not find layer:{inViewConfig.Layer}, WindowID:{inViewConfig.ID}");
            }
        }

        private void __windowSetupCanvas(UIFacade inFacade)
        {
            var canvas = inFacade.gameObject.GetOrAddComponent<Canvas>();
            var graphicRaycaster = inFacade.gameObject.GetOrAddComponent<GraphicRaycaster>();
            canvas.renderMode = UnityEngine.RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = this.UICamera;
            canvas.overrideSorting = true;
        }

        private void __windowSetupRectTransform(UIFacade inFacade)
        {
            var rectTransform = inFacade.gameObject.GetOrAddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(0, 0);
            rectTransform.localScale = new Vector3(1, 1, 1);
            rectTransform.localRotation = Quaternion.identity;
        }
    }
}
