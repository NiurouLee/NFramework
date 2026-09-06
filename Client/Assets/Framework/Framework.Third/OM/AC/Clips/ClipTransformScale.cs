using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Transform/Scale", "Animaite Scale")]
    public class ClipTransformScale : ACTweenClip<Vector3,Transform>
    {
        protected override Vector3 GetCurrentValue()
        {
            return target.localScale;
        }

        protected override void SetValue(Vector3 newValue)
        {
            target.localScale = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(Vector3.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}