using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.AC.Editor
{
    /// <summary>
    /// right handle of the clip to change the duration
    /// </summary>
    public class ACTimelineClipRightHandle : VisualElement,IACDraggable
    {
        private readonly ACTimelineClip _clip;
        private float _startDuration;

        
        public ACTimelineClipRightHandle(ACTimelineClip clip)
        {
            _clip = clip;
            AddToClassList("clip-handle");
        }

        public void StartDrag(Vector2 mousePosition)
        {
            _clip.StartDrag(mousePosition);
            _startDuration = _clip.Clip.Duration;
        }

        public bool Drag(Vector2 delta,Vector2 mousePosition)
        {
            var newDuration = _startDuration + delta.x / _clip.Timeline.GetWidthPerSecond();
            Undo.RecordObject(_clip.Timeline.Animator, "Change Clip Duration");
            _clip.Clip.Duration = Mathf.Clamp(newDuration,0,_clip.Timeline.Animator.FullDuration - _clip.Clip.StartAt);
            return false;
        }

        public void EndDrag(Vector2 delta,Vector2 mousePosition)
        {
            _clip.EndDrag(delta,mousePosition);
        }
        
        public static bool IsWithinRange(float value, float target, float range)
        {
            return Mathf.Abs(value - target) <= range;
        }
    }
}