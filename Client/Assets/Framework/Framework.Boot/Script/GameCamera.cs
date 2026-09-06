using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace NFramework.Boot
{
    public class GameCamera : MonoBehaviour
    {
        public static GameCamera Instance { get; private set; }
        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            this.m_MainCamera = GetComponent<Camera>();
            DontDestroyOnLoad(gameObject);
        }

        public Camera Main => m_MainCamera;

        [SerializeField] private Camera m_MainCamera;

        public void Stack(Camera camera, int index)
        {
            var main = m_MainCamera.GetUniversalAdditionalCameraData();
            var cameraStack = main.cameraStack;
            // check if camera is already in stack
            if (cameraStack.Contains(camera))
            {
                cameraStack.Remove(camera);
            }

            cameraStack.Insert(index, camera);
            OnStack(camera);
        }

        public void Stack(Camera camera)
        {
            var main = m_MainCamera.GetUniversalAdditionalCameraData();
            var cameraStack = main.cameraStack;
            // check if camera is already in stack
            if (cameraStack.Contains(camera))
            {
                return;
            }
            cameraStack.Add(camera);
            OnStack(camera);
        }

        public void Unstack(Camera camera)
        {
            var main = m_MainCamera.GetUniversalAdditionalCameraData();
            main.cameraStack.Remove(camera);
        }

        private void OnStack(Camera camera)
        {
            var cameraData = camera.GetUniversalAdditionalCameraData();
            cameraData.renderType = CameraRenderType.Overlay;
            camera.rect = m_MainCamera.rect;
        }


    }
}
