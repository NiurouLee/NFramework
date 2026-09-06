using OM.AC;
using UnityEngine;

namespace Framework.ModuleSystem
{
    public class OMWindowAnimation : MonoBehaviour, IwindowCloseAnim, IWindowOpenAnim
    {
        [SerializeField] ACAnimatorPlayer OpenAnim;
        [SerializeField] ACAnimatorPlayer CloseAnim;

        public float CloseAnimTime => CloseAnim ? CloseAnim.FullDuration : 0;

        public void PlayClose()
        {
            CloseAnim?.Play();
        }

        public void PauseClose()
        {
            CloseAnim?.Pause();
        }

        public float OpenAnimTime => OpenAnim ? OpenAnim.FullDuration : 0;

        public void PlayOpen()
        {
            OpenAnim?.Play();
        }

        public void PauseOpen()
        {
            OpenAnim?.Pause();
        }
    }
}