using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.AC.Editor
{
    /// <summary>
    /// Timeline Time Section to display the current time, and the seconds
    /// </summary>
    public class ACTimelineTimeSection : VisualElement,IACUpdateable,IACDraggable
    {
        private readonly ACTimeline _timeline;
        private readonly VisualElement _pointer;
        private readonly VisualElement _pointerLine;
        private readonly List<Label> _numbers = new List<Label>();

        
        public ACTimelineTimeSection(ACTimeline timeline)
        {
            _timeline = timeline;
            AddToClassList("time-section");
            
            var step = _timeline.Animator.FullDuration / 10;

            for (var i = 0; i <= 10; i++)
            {
                var number = new Label((i * step).ToString("0.00"))
                {
                    pickingMode = PickingMode.Ignore
                };
                number.AddToClassList("time-section-number");
                Add(number);
                _numbers.Add(number);
            }
            
            _pointer = new VisualElement();
            _pointer.AddToClassList("time-section-pointer");
            _pointer.pickingMode = PickingMode.Ignore;
            Add(_pointer);

            var pointerIcon = new VisualElement();
            pointerIcon.name = "pointer-icon";
            pointerIcon.pickingMode = PickingMode.Ignore;
            pointerIcon.style.backgroundImage = (StyleBackground)EditorGUIUtility.IconContent("d_Animation.EventMarker").image;
            _pointer.Add(pointerIcon);
            
            _pointerLine = new VisualElement();
            _pointerLine.name = "pointer-line";
            _pointerLine.AddToClassList("pointer-line");
            _pointerLine.pickingMode = PickingMode.Ignore;
            _timeline.TopContainer.Add(_pointerLine);
        }
        
        public void Update()
        {
            var widthPerSecond = _timeline.GetWidthPerSecond(_timeline.Animator.TimelineTime);
            _pointer.style.left = Mathf.Max(0,widthPerSecond * _timeline.GetTimelineWidth()) ;

            var step = _timeline.Animator.FullDuration / 10;
            for (var i = 0; i < _numbers.Count; i++)
            {
                _numbers[i].text = (i * step).ToString("0.00");
            }
            
            _pointerLine.style.left = _pointer.layout.x;
            _pointerLine.style.top = _timeline.TopContainer.WorldToLocal(this.LocalToWorld(_pointer.layout.position)).y;
            _pointerLine.style.height = _timeline.ContentSection.layout.height + layout.height;
            
        }

        public void StartDrag(Vector2 mousePosition)
        {
            
        }

        public bool Drag(Vector2 delta, Vector2 mousePosition)
        {
            if (!_timeline.AnimatorEditor.IsPreviewInstance()) return false;
            
            var secondsPerWidth = _timeline.GetSecondsPerWidth();
            _timeline.Animator.Evaluate((mousePosition.x - layout.x) * secondsPerWidth,true);
            return false;
        }

        public void EndDrag(Vector2 delta, Vector2 mousePosition)
        {
        }
    }
}