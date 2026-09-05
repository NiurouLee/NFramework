using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("RectTransform/Offset Min", "Offset Min")]
    public class ClipRectTransformOffsetMin : ACTweenClip<Vector2,RectTransform>
    {
        protected override Vector2 GetCurrentValue()
        {
            return target.offsetMin;
        }

        protected override void SetValue(Vector2 newValue)
        {
            target.offsetMin = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(Vector2.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}