using OM.Shared;
using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("AC Animator/Timeline", "Custom Timeline")]
    public class ClipAnimatorTimeline : ACClip
    {
        [SerializeField,CheckForNull] private ACAnimator target;
        
        protected override void OnStateChanged(ACEvaluateState currentState, ACEvaluateState lastState) { }

        protected override void OnEnter()
        {
            foreach (var clip in target.GetClips())
            {
                clip.OnTimelineStarted();
            }
        }

        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            target.Evaluate(normalizedClipTime * target.FullDuration,previewMode);
        }

        protected override void OnExit()
        {
            foreach (var clip in target.GetClips())
            {
                clip.OnTimelineCompleted();
            }
        }

        public override void OnTimelineStarted()
        {
            
        }

        public override void OnTimelineCompleted()
        {
            
        }

        public override void OnPreviewModeChanged(bool previewMode)
        {
            if (!IsValid()) return;
            foreach (var acClip in target.GetClips())
            {
                acClip.OnPreviewModeChanged(previewMode);
            }
        }

        public override void OnClipAddedToACAnimator(ACAnimator animator)
        {
        }

        public override void OnAnimatorStartPlaying()
        {
        }

        public override void OnAnimatorCompletePlaying()
        {
        }

        public override bool CanBePlayedInPreviewMode()
        {
            return true;
        }

        public override bool IsValid()
        {
            return target != null;
        }

        [OMCustomButton("Reset Timeline Duration")]
        public void CustomButton()
        {
            if(!IsValid()) return;
            Duration = target.FullDuration;
        }

        public override Component GetTarget()
        {
            return target;
        }

        public override void SetTarget(GameObject newTarget)
        {
            this.target = newTarget.GetComponent<ACAnimator>();
        }
    }
}