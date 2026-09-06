using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Transform/Position", "Move Position")]
    public class ClipTransformPosition : ACTweenClip<Vector3,Transform>
    {
        [SerializeField] private bool local;
        protected override Vector3 GetCurrentValue()
        {
            return local? target.localPosition : target.position;
        }

        protected override void SetValue(Vector3 newValue)
        {
            if(local) target.localPosition = newValue;
            else target.position = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(Vector3.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}