using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Light/Light Intensity", "Animate Light Intensity")]
    public class ClipLightIntensity : ACTweenClip<float,Light>
    {
        protected override float GetCurrentValue()
        {
            return target.intensity;
        }

        protected override void SetValue(float newValue)
        {
            target.intensity = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(Mathf.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}