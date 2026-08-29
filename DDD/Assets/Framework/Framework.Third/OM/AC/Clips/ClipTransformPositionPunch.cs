using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("Transform/Position Punch", "Position Punch")]
    public class ClipTransformPositionPunch : ACPunchCore<Vector3,Transform>
    {
        [SerializeField] private bool local;
        
        protected override Vector3 GetCurrentValue()
        {
            return local ? target.localPosition : target.position;
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
            var damper = 1f - Mathf.Clamp(2f * normalizedClipTime - 1f, 0f, 1f);
            var bounce = Mathf.Sin(normalizedClipTime * Mathf.PI * vibrato) * elasticity;
            var newPosition = punchStrength * (damper * bounce);
            SetValue(startValue + newPosition);
        }
    }
}