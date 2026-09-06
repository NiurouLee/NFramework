using OM.Shared;
using UnityEngine;

namespace OM.AC.Demos
{
    [System.Serializable]
    [ACClipCreate("Custom/Custom Animation", "Custom Animation Clip")]
    public class CustomAnimationClip : ACClip
    {
        [SerializeField,CheckForNull] private Transform target;

        private Vector3 _startPos;
        private Quaternion _startRotation;
        private Vector3 _startScale;

        protected override void OnEnter()
        {
            base.OnEnter();
            _startPos = target.position;
            _startRotation = target.rotation;
            _startScale = target.localScale;
        }

        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime, float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            var t = normalizedClipTime;
            target.position = Vector3.LerpUnclamped(_startPos, _startPos + Vector3.up * 1.25f, t);
            target.rotation = Quaternion.LerpUnclamped(_startRotation, _startRotation * Quaternion.Euler(0f, 180f, 0f), t);
            target.localScale = Vector3.LerpUnclamped(_startScale, _startScale * 1.2f, t);
        }

        public override void OnPreviewModeChanged(bool previewMode)
        {
            
        }

        public override bool CanBePlayedInPreviewMode()
        {
            return false;
        }

        public override bool IsValid()
        {
            return target != null;
        }

        public override Component GetTarget()
        {
            return target;
        }

        public override void SetTarget(GameObject newTarget)
        {
            target = newTarget.transform;
        }
    }
}