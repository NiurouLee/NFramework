using GameObject = UnityEngine.GameObject;
using Vector3 = UnityEngine.Vector3;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;


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
        private UIFixedLayerServices m_FixedLayer;
        private UIStackLayerServices m_StackLayer;
        private WindowPoolServices m_Pool;


        public override void Awake()
        {
            GetSystem<LoggerSystem>().Error?.Print("UIM Awake");
            base.Awake();
            var _go = UnityEngine.Resources.Load<GameObject>("UIROOT");
            var _root = UnityEngine.Object.Instantiate(_go);
            this.AwakeRoot(_root);
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
            var fixedLayerGo = new GameObject("FixedLayer");
            var fixedLayerRectTransform = fixedLayerGo.AddComponent<RectTransform>();
            fixedLayerRectTransform.SetParent(this.UICanvasTrf);
            fixedLayerRectTransform.localPosition = new Vector3(0, 0, 0);
            fixedLayerRectTransform.localScale = new Vector3(1, 1, 1);
            fixedLayerRectTransform.localRotation = Quaternion.identity;
            fixedLayerRectTransform.anchorMin = new Vector2(0, 0);
            fixedLayerRectTransform.anchorMax = new Vector2(1, 1);
            fixedLayerRectTransform.pivot = new Vector2(0.5f, 0.5f);
            fixedLayerRectTransform.anchoredPosition = new Vector2(0, 0);
            fixedLayerRectTransform.sizeDelta = new Vector2(0, 0);
            this.m_FixedLayer = new UIFixedLayerServices(this, fixedLayerGo);


            var stackLayerGo = new GameObject("StackLayer");
            var stackLayerRectTransform = stackLayerGo.AddComponent<RectTransform>();
            stackLayerRectTransform.SetParent(this.UICanvasTrf);
            stackLayerRectTransform.localPosition = new Vector3(0, 0, 0);
            stackLayerRectTransform.localScale = new Vector3(1, 1, 1);
            stackLayerRectTransform.anchorMin = new Vector2(0, 0);
            stackLayerRectTransform.anchorMax = new Vector2(1, 1);
            stackLayerRectTransform.pivot = new Vector2(0.5f, 0.5f);
            stackLayerRectTransform.anchoredPosition = new Vector2(0, 0);
            stackLayerRectTransform.sizeDelta = new Vector2(0, 0);
            stackLayerRectTransform.localRotation = Quaternion.identity;
            this.m_StackLayer = new UIStackLayerServices(this, stackLayerGo);
        }


        private void AwakePoolServices()
        {
            this.m_Pool = new WindowPoolServices();
        }

        public void __WindowSetUpLayer(ViewConfig inViewConfig, Window inWindow, UIFacade inFacade)
        {
            if (inViewConfig.IsFixedLayer)
            {
                this.m_FixedLayer.PushWindow(inWindow, inViewConfig, inFacade);
            }
            else
            {
                this.m_StackLayer.PushWindow(inWindow, inViewConfig, inFacade);
            }
        }

        private void __windowSetupCanvas(UIFacade inFacade)
        {
            var canvas = inFacade.gameObject.GetOrAddComponent<UnityEngine.Canvas>();
            var graphicRaycaster = inFacade.gameObject.GetOrAddComponent<UnityEngine.UI.GraphicRaycaster>();
            canvas.renderMode = UnityEngine.RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = this.UICamera;
            canvas.overrideSorting = true;
        }

        private void __windowSetupRectTransform(UIFacade inFacade)
        {
            var rectTransform = inFacade.gameObject.GetOrAddComponent<UnityEngine.RectTransform>();
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