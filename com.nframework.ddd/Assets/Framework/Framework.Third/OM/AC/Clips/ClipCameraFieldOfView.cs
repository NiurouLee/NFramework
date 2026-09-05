using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Camera/Field Of View", "Camera Field Of View")]
    public class ClipCameraFieldOfView : ACTweenClip<float,Camera>
    {
        protected override float GetCurrentValue()
        {
            return target.fieldOfView;
        }

        protected override void SetValue(float newValue)
        {
            target.fieldOfView = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(Mathf.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}