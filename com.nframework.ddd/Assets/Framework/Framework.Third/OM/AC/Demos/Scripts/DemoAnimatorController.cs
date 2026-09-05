using System;
using UnityEngine;
using UnityEngine.UI;

namespace OM.AC.Demos
{
    public class DemoAnimatorController : MonoBehaviour
    {
        [SerializeField] private ACAnimator animator;
        [SerializeField] private Slider slider;

        private void Awake()
        {
            slider.minValue = 0;
            slider.maxValue = animator.FullDuration;
        }

        private void Update()
        {
            animator.Evaluate(slider.value,false);
        }
    }
}