using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.AC.Editor
{
    /// <summary>
    /// Left Handle of the clip to change the start time and duration
    /// </summary>
    public class ACTimelineClipLeftHandle : VisualElement,IACDraggable
    {
        private readonly ACTimelineClip _clip;
        private float _startDuration;
        private float _startAtTime;
        private float _startPosMinX;
        private float _startPosMaxX;
        
        public ACTimelineClipLeftHandle(ACTimelineClip clip)
        {
            _clip = clip;
            AddToClassList("clip-handle");
        }

        public void StartDrag(Vector2 mousePosition)
        {
            _clip.StartDrag(mousePosition);
            _startDuration = _clip.Clip.Duration;
            _startAtTime = _clip.Clip.StartAt;
            
            _startPosMinX = _clip.Timeline.GetWidthPerSecond() * _clip.Clip.StartAt;
            _startPosMaxX = _clip.Timeline.GetWidthPerSecond() * (_clip.Clip.GetEndTime());
        }

        public bool Drag(Vector2 delta,Vector2 mousePosition)
        {
            var newStartMinX = Mathf.Clamp(_startPosMinX + delta.x,0,_startPosMaxX);
            var newStartAt = newStartMinX / _clip.Timeline.GetWidthPerSecond();
            var newDuration = _startDuration - newStartAt + _startAtTime;

            
            Undo.RecordObject(_clip.Timeline.Animator, "Change Clip Duration and StartAt");
            _clip.Clip.StartAt = newStartAt;
            _clip.Clip.Duration = newDuration;

            return false;
        }

        public void EndDrag(Vector2 delta,Vector2 mousePosition)
        {
            _clip.EndDrag(delta,mousePosition);
        }
    }
}