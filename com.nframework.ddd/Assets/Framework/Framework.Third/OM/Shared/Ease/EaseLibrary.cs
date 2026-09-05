using System;
using UnityEngine;

namespace OM.Ease
{
    public static class EaseLibrary
    {
        public static Func<float,float> GetLerpMethod(EaseType easeType)
        {
            return easeType switch
            {
                EaseType.Linear => EaseLinear,
                
                EaseType.InSine => EaseInSine,
                EaseType.OutSine => EaseOutSine,
                EaseType.InOutSine => EaseInOutSine,
                
                EaseType.InQuad => EaseInQuad,
                EaseType.OutQuad => EaseOutQuad,
                EaseType.InOutQuad => EaseInOutQuad,
                
                EaseType.InBack => EaseInBack,
                EaseType.OutBack => EaseOutBack,
                EaseType.InOutBack => EaseInOutBack,
                
                EaseType.InCubic => EaseInCubic,
                EaseType.OutCubic => EaseOutCubic,
                EaseType.InOutCubic => EaseInOutCubic,

                EaseType.InBounce => EaseInBounce,
                EaseType.OutBounce => EaseOutBounce,
                EaseType.InOutBounce => EaseInOutBounce,
                
                EaseType.InQuint => EaseInQuint,
                EaseType.OutQuint => EaseOutQuint,
                EaseType.InOutQuint => EaseInOutQuint,
                
                EaseType.InCirc => EaseInCirc,
                EaseType.OutCirc => EaseOutCirc,
                EaseType.InOutCirc => EaseInOutCirc,
                
                EaseType.InElastic => EaseInElastic,
                EaseType.OutElastic => EaseOutElastic,
                EaseType.InOutElastic => EaseInOutElastic,
                
                EaseType.InExpo => EaseInExpo,
                EaseType.OutExpo => EaseOutExpo,
                EaseType.InOutExpo => EaseInOutExpo,
                
                EaseType.InQuart => EaseInQuart,
                EaseType.OutQuart => EaseOutQuart,
                EaseType.InOutQuart => EaseInOutQuart,
                _ => EaseLinear
            };
        }

        public static float LerpEase(float t, EaseType easeType)
        {
            return easeType switch
            {
                EaseType.Linear => EaseLinear(t),
                
                EaseType.InSine => EaseInSine(t),
                EaseType.OutSine => EaseOutSine(t),
                EaseType.InOutSine => EaseInOutSine(t),
                
                EaseType.InQuad => EaseInQuad(t),
                EaseType.OutQuad => EaseOutQuad(t),
                EaseType.InOutQuad => EaseInOutQuad(t),
                
                EaseType.InBack => EaseInBack(t),
                EaseType.OutBack => EaseOutBack(t),
                EaseType.InOutBack => EaseInOutBack(t),
                
                EaseType.InCubic => EaseInCubic(t),
                EaseType.OutCubic => EaseOutCubic(t),
                EaseType.InOutCubic => EaseInOutCubic(t),

                EaseType.InBounce => EaseInBounce(t),
                EaseType.OutBounce => EaseOutBounce(t),
                EaseType.InOutBounce => EaseInOutBounce(t),
                
                EaseType.InQuint => EaseInQuint(t),
                EaseType.OutQuint => EaseOutQuint(t),
                EaseType.InOutQuint => EaseInOutQuint(t),
                
                EaseType.InCirc => EaseInCirc(t),
                EaseType.OutCirc => EaseOutCirc(t),
                EaseType.InOutCirc => EaseInOutCirc(t),
                
                EaseType.InElastic => EaseInElastic(t),
                EaseType.OutElastic => EaseOutElastic(t),
                EaseType.InOutElastic => EaseInOutElastic(t),
                
                EaseType.InExpo => EaseInExpo(t),
                EaseType.OutExpo => EaseOutExpo(t),
                EaseType.InOutExpo => EaseInOutExpo(t),
                
                EaseType.InQuart => EaseInQuart(t),
                EaseType.OutQuart => EaseOutQuart(t),
                EaseType.InOutQuart => EaseInOutQuart(t),
                
                _ => 0f
            };
        }

        public static float EaseLinear(float t)
        {
            return Mathf.LerpUnclamped(0, 1, t);
        }
    
        //Sine
        public static float EaseInSine(float t)
        {
            return 1 - Mathf.Cos((t * Mathf.PI) / 2);
        }
        public static float EaseOutSine(float t)
        {
            return Mathf.Sin((t * Mathf.PI) / 2);
        }

        public static float EaseInOutSine(float t)
        {
            return -(Mathf.Cos(t * Mathf.PI) - 1) / 2;
        }
    
        //Cubic
        public static float EaseInCubic(float t)
        {
            return t * t * t;
        }
        public static float EaseOutCubic(float t)
        {
            return 1 - Mathf.Pow(1 - t, 3);
        }

        public static float EaseInOutCubic(float t)
        {
            if (t < 0.5f)
            {
                return 4 * t * t * t;
            }

            var f = 2 * t - 2;
            return 0.5f * f * f * f + 1;
        }
    
        //Quint
        public static float EaseInQuint(float t)
        {
            return t * t * t * t * t;
        }
        public static float EaseOutQuint(float t)
        {
            return 1 - Mathf.Pow(1 - t, 5);
        }
        public static float EaseInOutQuint(float t)
        {
            if (t < 0.5)
            {
                return 16 * t * t * t * t * t;
            }

            var f = (2 * t - 2);
            return 0.5f * f * f * f * f * f + 1;
        }
        
        //Circ
        public static float EaseInCirc(float t)
        {
            return 1 - Mathf.Sqrt(1 - t * t);
        }

        public static float EaseOutCirc(float t)
        {
            return Mathf.Sqrt(1 - (t - 1) * (t - 1));
        }
        public static float EaseInOutCirc(float t)
        {
            if (t < 0.5)
            {
                return 0.5f * (1 - Mathf.Sqrt(1 - 4 * t * t));
            }
            return 0.5f * (Mathf.Sqrt(1 - (2 * t - 2) * (2 * t - 2)) + 1);
        }
        
        //Elastic
        public static float EaseInElastic(float t)
        {
            if (t is 0 or 1)
            {
                return t;
            }

            const float period = .3f;
            const float amplitude = 1;
            
            var s = period / 4;
            var p = amplitude * Mathf.Pow(2, 10 * (t - 1));
            return -(p * Mathf.Sin((t - 1 - s) * (2 * Mathf.PI) / period));
        }
        public static float EaseOutElastic(float t)
        {
            if (t is 0 or >= 1)
            {
                return t;
            }
            
            const float period = .3f;
            const float amplitude = 1;

            var s = period / (2 * Mathf.PI) * Mathf.Asin(1 / amplitude);
            return amplitude * Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / period) + 1;
        }
        public static float EaseInOutElastic(float t)
        {
            if (t == 0)
            {
                return 0;
            }

            if (t >= 1)
            {
                return 1;
            }
            const float period = .3f;
            const float amplitude = 1;
            
            var s = period / (2 * Mathf.PI) * Mathf.Asin(1 / amplitude);

            if (t < 0.5)
            {
                t = 2 * t;
                return -0.5f * (amplitude * Mathf.Pow(2, 10 * (t - 1)) * Mathf.Sin((t - 1 - s) * (2 * Mathf.PI) / period));
            }

            t = 2 * t - 1;
            return 0.5f * (amplitude * Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / period)) + 1;
        }
        
        //Quart
        public static float EaseInQuart(float t)
        {
            return t * t * t * t;
        }

        public static float EaseOutQuart(float t)
        {
            return 1f - Mathf.Pow(1f - t, 4);
        }

        public static float EaseInOutQuart(float t)
        {
            if (t < 0.5f)
            {
                return 8f * Mathf.Pow(t, 4);
            }

            var f = t - 1f;
            return -8f * Mathf.Pow(f, 4) + 1f;
        }
        
        //Expo
        public static float EaseInExpo(float t)
        {
            return (t == 0f) ? 0f : Mathf.Pow(2f, 10f * (t - 1f));
        }

        public static float EaseOutExpo(float t)
        {
            return (t >= 1f) ? 1f : 1f - Mathf.Pow(2f, -10f * t);
        }
        public static float EaseInOutExpo(float t)
        {
            if (t == 0f)
            {
                return 0f;
            }

            if (t >= 1f)
            {
                return 1f;
            }

            if (t < 0.5f)
            {
                return 0.5f * Mathf.Pow(2f, 20f * t - 10f);
            }

            return 1f - 0.5f * Mathf.Pow(2f, -20f * t + 10f);
        }

        //Quad
        public static float EaseInQuad(float t)
        {
            return t * t;
        }

        public static float EaseOutQuad(float t)
        {
            return 1f - (1f - t) * (1f - t);
        }

        public static float EaseInOutQuad(float t)
        {
            if (t < 0.5f)
            {
                return 2f * t * t;
            }

            return 1f - 2f * (1f - t) * (1f - t);
        }
    
        //Back
        public static float EaseInBack(float t)
        {
            const float overshoot = 1.70158f;
            return t * t * ((overshoot + 1f) * t - overshoot);
        }
        public static float EaseOutBack(float t)
        {
            const float overshoot = 1.70158f;
            t -= 1f; // Start at t = 1
            return 1f + t * t * ((overshoot + 1f) * t + overshoot);
        }
        public static float EaseInOutBack(float t)
        {
            const float overshoot = 1.70158f;

            t *= 2f;
            if (t < 1f)
            {
                //overshoot *= 1.525f; // Optional: Adjust the overshoot value for the ease-in part
                return 0.5f * (t * t * ((overshoot + 1f) * t - overshoot));
            }

            t -= 2f; // Continue from t = 2
            return 0.5f * (t * t * ((overshoot + 1f) * t + overshoot)) + 1f;
        }

        //Bounce
        public static float EaseInBounce(float t)
        {
            return 1f - EaseOutBounce(1f - t);
        }

        public static float EaseOutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1 / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2 / d1)
            {
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            }
            else if (t < 2.5 / d1)
            {
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            }
            else
            {
                return n1 * (t -= 2.625f / d1) * t + 0.984375f;
            }
        }
        
        public static float EaseInOutBounce(float t)
        {
            if (t < 0.5f)
            {
                return 0.5f * EaseInBounce(t * 2f);
            }

            return 0.5f * EaseOutBounce(t * 2f - 1f) + 0.5f;
        }
    
    }
}