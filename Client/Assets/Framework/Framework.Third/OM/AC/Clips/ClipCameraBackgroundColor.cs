using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Camera/Background Color", "Camera Background Color")]
    public class ClipCameraBackgroundColor : ACTweenClip<Color,Camera>
    {
        protected override Color GetCurrentValue()
        {
            return target.backgroundColor;
        }

        protected override void SetValue(Color newValue)
        {
            target.backgroundColor = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(Color.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}