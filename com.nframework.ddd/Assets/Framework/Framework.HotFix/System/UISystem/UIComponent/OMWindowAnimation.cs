using UnityEngine;

namespace Framework.ModuleSystem
{
    public class OMWindowAnimation : MonoBehaviour, IwindowCloseAnim, IWindowOpenAnim
    {
        
        
        public float CloseAnimTime { get; }

        public void PlayClose()
        {
        }

        public void PauseClose()
        {
        }

        public float OpenAnimTime { get; }

        public void PlayOpen()
        {
        }

        public void PauseOpen()
        {
        }
    }
}
