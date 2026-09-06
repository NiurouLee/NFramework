using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Transform/Rotation", "Animate Rotation")]
    public class ClipTransformRotation : ACTweenClip<Vector3,Transform>
    {
        [SerializeField] private bool local;
        
        protected override Vector3 GetCurrentValue()
        {
            return local? target.localRotation.eulerAngles : target.rotation.eulerAngles;
        }

        protected override void SetValue(Vector3 newValue)
        {
            if(local) target.localRotation = Quaternion.Euler(newValue);
            else target.rotation = Quaternion.Euler(newValue);
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(Vector3.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}