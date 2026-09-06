using UnityEngine;
using UnityEngine.UIElements;

namespace OM.AC.Editor
{
    public interface IACDraggable
    {
        void StartDrag(Vector2 mousePosition);
        bool Drag(Vector2 delta, Vector2 mousePosition);
        void EndDrag(Vector2 delta, Vector2 mousePosition);
    }
    
    public interface IACClickable
    {
        void Click(MouseButton mouseButton);
    }
    
    public class ACTimelineManipulator : MouseManipulator
    {
        private readonly ACTimeline _timeline;
        private bool _isActive = false;
        private Vector2 _startMousePos;
        private Vector2 _startOffset;
        private float _startWidth;
        private float _startTime;
        private IACDraggable _draggable;
        private IACClickable _clickable;
        
        public ACTimelineManipulator(ACTimeline timeline)
        {
            _timeline = timeline;
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            this.target.RegisterCallback<MouseDownEvent>(this.OnMouseDown);
            this.target.RegisterCallback<MouseMoveEvent>(this.OnMouseMove);
            this.target.RegisterCallback<MouseUpEvent>(this.OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            this.target.UnregisterCallback<MouseDownEvent>(this.OnMouseDown);
            this.target.UnregisterCallback<MouseMoveEvent>(this.OnMouseMove);
            this.target.UnregisterCallback<MouseUpEvent>(this.OnMouseUp);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            _startMousePos = e.localMousePosition;
            _startTime = Time.realtimeSinceStartup;

            if (_isActive)
            {
                e.StopImmediatePropagation();
            }
            else if (e.target is VisualElement visualElement)
            {
                if ((MouseButton)e.button == MouseButton.LeftMouse && visualElement is IACDraggable draggable)
                {
                    target.CaptureMouse();
                    e.StopPropagation();
                    _isActive = true;
                    draggable.StartDrag(e.localMousePosition);
                    _draggable = draggable;   
                }
                if(visualElement is IACClickable clickable)
                {
                    _clickable = clickable;
                }
            }
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            if(!_isActive) return;
            
            if(_draggable == null) return;
            var mouseDelta = e.localMousePosition - _startMousePos;
            _draggable.Drag(mouseDelta,e.localMousePosition);
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if(Time.realtimeSinceStartup - _startTime < 0.35f && (e.localMousePosition - _startMousePos).magnitude < 6f)
            {
                if (_clickable != null)
                {
                    _clickable.Click((MouseButton)e.button);
                }
            }
            
            _clickable = null;
            if(!_isActive) return;
            if(_draggable == null) return;
            
            _draggable.EndDrag(e.localMousePosition - _startMousePos,e.localMousePosition);
            _draggable = null;
            
            target.ReleaseMouse();
            e.StopPropagation();
            _isActive = false;
        }
        
    }
}