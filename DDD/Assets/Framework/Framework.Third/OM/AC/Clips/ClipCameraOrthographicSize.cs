using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Camera/Orthographic Size", "Camera Orthographic Size")]
    public class ClipCameraOrthographicSize : ACTweenClip<float,Camera>
    {
        protected override float GetCurrentValue()
        {
            return target.orthographicSize;
        }

        protected override void SetValue(float newValue)
        {
            target.orthographicSize = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(Mathf.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}