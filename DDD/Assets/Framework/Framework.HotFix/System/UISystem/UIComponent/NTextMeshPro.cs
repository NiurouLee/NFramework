using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace NFramework.ModuleSystem
{
    public class NTextMeshPro : TextMeshProUGUI
    {
        [SerializeField] private Color32 customOutLineColor;
        [SerializeField, Range(0f, 1f)] private float customOutLineWidth;
        [SerializeField] private string languageKey;

        public string LanguageKey
        {
            get { return languageKey; }
            set
            {
                languageKey = value;
                text = GetLanguageStr?.Invoke(languageKey);
            }
        }

        [SerializeField] private bool shadow;
        [SerializeField] private Color shadowColor = new Color(0, 0, 0, 1f);
        [SerializeField, Range(-1f, 1f)] private float offsetX = 0;
        [SerializeField, Range(-1f, 1f)] private float offsetY = 0.5f;
        [SerializeField, Range(-1f, 1f)] private float dilate = 0.1f;
        [SerializeField, Range(0f, 1f)] private float softness = 0;
        [SerializeField, Range(-1f, 1f)] private float faceDilate = 0;
        private static readonly int underlayColor = Shader.PropertyToID("_UnderlayColor");
        private static readonly int underlayOffsetY = Shader.PropertyToID("_UnderlayOffsetY");
        private static readonly int underlayOffsetX = Shader.PropertyToID("_UnderlayOffsetX");
        private static readonly int underlayDilate = Shader.PropertyToID("_UnderlayDilate");
        private static readonly int underlaySoftness = Shader.PropertyToID("_UnderlaySoftness");
        private static readonly int FaceDilate = Shader.PropertyToID("_FaceDilate");

        public static event Func<string, string> GetLanguageStr;

        protected override void Awake()
        {
            base.Awake();
            if (GetLanguageStr != null && !string.IsNullOrEmpty(languageKey)) text = GetLanguageStr(languageKey);
            if (Application.isPlaying)
                CheckNewMat();
            Refresh();
        }

        public void Refresh()
        {
            if (m_sharedMaterial == null) return;
            if (m_fontMaterial == null)
            {
                m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
                m_sharedMaterial = m_fontMaterial;
            }

            m_fontMaterial.SetFloat(FaceDilate, faceDilate);
            ShowShadow();
            ShowOutline();
        }

        private void ShowShadow()
        {
            if (shadow)
            {
                m_fontMaterial.EnableKeyword("UNDERLAY_ON");
                m_fontMaterial.SetColor(underlayColor, shadowColor);
                m_fontMaterial.SetFloat(underlayOffsetX, offsetX);
                m_fontMaterial.SetFloat(underlayOffsetY, -offsetY);
                m_fontMaterial.SetFloat(underlayDilate, dilate);
                m_fontMaterial.SetFloat(underlaySoftness, softness);
            }
            else
            {
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
                m_fontMaterial.DisableKeyword("UNDERLAY_ON");
#endif
            }
        }

        private void ShowOutline()
        {
            if (customOutLineColor.a > 0 && customOutLineWidth > 0)
            {
                m_fontMaterial.EnableKeyword("OUTLINE_ON");
            }

            var lineColor = customOutLineColor;
            lineColor.r = (byte)(Mathf.GammaToLinearSpace(customOutLineColor.r / 255f) * 255);
            lineColor.g = (byte)(Mathf.GammaToLinearSpace(customOutLineColor.g / 255f) * 255);
            lineColor.b = (byte)(Mathf.GammaToLinearSpace(customOutLineColor.b / 255f) * 255);
            lineColor.a = (byte)(Mathf.GammaToLinearSpace(customOutLineColor.a / 255f) * 255);
            outlineColor = lineColor;
            outlineWidth = customOutLineWidth;
        }

        private static Dictionary<long, Material> hash2Mat;
        private static Dictionary<long, int> hash2MatCount;
        private static Material defaultMat;
        public long CreateHash = -1;
        private async Task CheckNewMat()
        {
            if (hash2Mat == null)
            {
                hash2Mat = new Dictionary<long, Material>();
                hash2MatCount = new Dictionary<long, int>();
                if (defaultMat == null)
                {
                    defaultMat = m_sharedMaterial;
                }
            }

            long hash = color.GetHashCode();
            hash += faceDilate.GetHashCode();
            if (shadow)
            {
                hash += shadowColor.GetHashCode() + offsetX.GetHashCode() + offsetY.GetHashCode() +
                        dilate.GetHashCode() +
                        softness.GetHashCode();
            }

            if (outlineColor.a > 0 && customOutLineWidth > 0)
            {
                hash += customOutLineColor.GetHashCode() + customOutLineWidth.GetHashCode();
            }
            CreateHash = hash;

            if (!hash2Mat.TryGetValue(hash, out var mat))
            {
                mat = CreateMaterialInstance(defaultMat);
                hash2Mat[hash] = mat;
                hash2MatCount[hash] = 0;
            }
            else
            {
                int nowCount = hash2MatCount[hash];
                nowCount++;
                hash2MatCount[hash] = nowCount;
            }


            m_fontMaterial = mat;
            m_sharedMaterial = m_fontMaterial;
        }

        private void ReleaseMat()
        {
            if (m_fontMaterial != null && hash2Mat != null)
            {
                long hash = CreateHash;


                if (hash2Mat.TryGetValue(hash, out var matEntry))
                {
                    int nowCount = hash2MatCount[hash];
                    nowCount--;
                    if (nowCount <= 0)
                    {
                        UnityEngine.Object.Destroy(matEntry);
                        hash2Mat.Remove(hash);
                        hash2MatCount.Remove(hash);
                    }
                    else
                    {
                        hash2MatCount[hash] = nowCount;
                    }
                }
            }
        }

    }
}