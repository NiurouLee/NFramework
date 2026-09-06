using UnityEngine;
using UnityEngine.EventSystems;

namespace OM.AC.Demos
{
    public class DemoButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField] private ACAnimatorPlayer animator;

        public void OnPointerEnter(PointerEventData eventData)
        {
            animator.Play();
            animator.Loop = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            animator.Loop = false;
        }
    }
}