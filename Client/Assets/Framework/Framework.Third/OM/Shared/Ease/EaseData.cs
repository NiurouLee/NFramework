using UnityEngine;

namespace OM.Ease
{
    public enum EaseDataType
    {
        Ease = 0,
        AnimationCurve = 1,
    }
    
    /// <summary>
    /// Ease Data Class to store the Ease Type and Animation Curve
    /// </summary>
    [System.Serializable]
    public class EaseData
    {
        public EaseDataType easeDataType;
        public AnimationCurve animationCurve = AnimationCurve.Linear(0,0,1,1);
        public EaseType easeType = EaseType.Linear;

        public EaseData()
        {
            easeDataType = EaseDataType.Ease;
            animationCurve = AnimationCurve.Linear(0,0,1,1);
            easeType = EaseType.Linear;
        }
        
        public EaseData(AnimationCurve animationCurve)
        {
            easeDataType = EaseDataType.AnimationCurve;
            this.animationCurve = animationCurve;
        }

        public EaseData(EaseType easeType)
        {
            easeDataType = EaseDataType.Ease;
            this.easeType = easeType;
        }

        /// <summary>
        /// Lerp the value using the ease type or animation curve
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float Lerp(float t)
        {
            return easeDataType == EaseDataType.Ease ? EaseLibrary.LerpEase(Mathf.Clamp01(t), easeType) : animationCurve.Evaluate(Mathf.Clamp01(t));
        }
        
    }
}