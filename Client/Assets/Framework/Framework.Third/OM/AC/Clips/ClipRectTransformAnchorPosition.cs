using UnityEngine;

namespace OM.AC
{
    [System.Serializable]
    [ACClipCreate("RectTransform/Anchor Position", "Move Anchor Position")]
    public class ClipRectTransformAnchorPosition : ACTweenClip<Vector3,RectTransform>
    {
        protected override Vector3 GetCurrentValue()
        {
            return target.anchoredPosition3D;
        }

        protected override void SetValue(Vector3 newValue)
        {
            target.anchoredPosition3D = newValue;
        }
        
        protected override void OnUpdate(ACEvaluateState state, float timelineTime, float clipTime,
            float normalizedClipTime, bool previewMode)
        {
            if(state != ACEvaluateState.Running) return;
            SetValue(Vector3.LerpUnclamped(CurrentFrom, to, ease.Lerp(normalizedClipTime)));
        }
    }
}