using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NFramework.ModuleSystem
{
    public class NGraphicRaycaster : GraphicRaycaster
    {
        public Camera TargetCamera;

        public override Camera eventCamera
        {
            get
            {
                if (TargetCamera == null)
                {
                    return NFROOT.Instance.GetSystem<UISystem>().UICamera;
                }

                return TargetCamera;
            }
        }

        private Canvas m_canvas;

        private Canvas canvas
        {
            get
            {
                if (m_canvas == null)
                    m_canvas = this.GetComponent<Canvas>();
                return m_canvas;
            }
        }

        [NonSerialized] private List<Graphic> m_RaycastResults = new List<Graphic>();

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (canvas == null)
            {
                return;
            }

            var canvasGraphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
            if (canvasGraphics == null || canvasGraphics.Count == 0)
                return;

            int displayIndex;

            var currentEventCamera = eventCamera;

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || currentEventCamera == null)
            {
                displayIndex = canvas.targetDisplay;
            }
            else
            {
                displayIndex = currentEventCamera.targetDisplay;
            }

            var eventPosition = Display.RelativeMouseAt(eventData.position);
            if (eventPosition != Vector3.zero)
            {
                int eventDisplayIndex = (int)eventPosition.z;
                if (eventDisplayIndex != displayIndex)
                    return;
            }
            else
            {
                eventPosition = eventData.position;
            }

            Vector2 pos;
            if (currentEventCamera == null)
            {
                float w = Screen.width;
                float h = Screen.height;
                if (displayIndex > 0 && displayIndex < Display.displays.Length)
                {
                    w = Display.displays[displayIndex].systemWidth;
                    h = Display.displays[displayIndex].systemHeight;
                }

                pos = new Vector2(eventPosition.x / w, eventPosition.y / h);
            }
            else
            {
                pos = currentEventCamera.WorldToScreenPoint(eventPosition);
            }

            if (pos.x < 0f || pos.x > 1f || pos.y < 0f || pos.y > 1f)
            {
                return;
            }

            float hitDistance = float.MaxValue;
            Ray ray = new Ray();
            if (currentEventCamera != null)
            {
                ray = currentEventCamera.ScreenPointToRay(eventPosition);
            }
        }
    }
}