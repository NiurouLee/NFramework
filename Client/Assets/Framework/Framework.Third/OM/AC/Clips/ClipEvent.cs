using UnityEngine;
using UnityEngine.Events;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("AC Animator/Event", "Event")]
    public class ClipEvent : ACClip
    {
        protected override void OnStateChanged(ACEvaluateState currentState, ACEvaluateState lastState) { }

        protected override void OnEnter() { }

        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime, float normalizedClipTime, bool previewMode) { }

        protected override void OnExit() { }

        public override void OnTimelineStarted() { }

        public override void OnTimelineCompleted() { }

        public override void OnPreviewModeChanged(bool previewMode) { }

        public override void OnClipAddedToACAnimator(ACAnimator animator) { }

        public override void OnAnimatorStartPlaying() { }

        public override void OnAnimatorCompletePlaying() { }

        public override bool CanBePlayedInPreviewMode()
        {
            return false;
        }
        
        public override bool IsValid()
        {
            return true;
        }
        
        public override Component GetTarget()
        {
            return null;
        }

        public override void SetTarget(GameObject newTarget)
        {
            
        }
    }
}