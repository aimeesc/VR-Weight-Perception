using UnityEngine;
using System.Collections;
using System.Linq;

namespace exiii.Unity
{
    [RequireComponent(typeof(Renderer))]
    public class WebcamCapture : MonoBehaviour
    {
        [SerializeField]
        private string m_TargetDevice;

        [SerializeField]
        private float m_verticalFOV = 85;

        [SerializeField]
        private int m_Width = 1920;

        [SerializeField]
        private int m_Hight = 1080;

        [SerializeField]
        private float m_Resolution = 1920f / 0.2f;

#pragma warning disable 414
        [SerializeField]
        private string[] m_Devices;
#pragma warning restore 414

        private WebCamTexture m_Webcamtex;
        private Camera m_Camera;

        private void Start()
        {
            m_Devices = WebCamTexture.devices.Select(x => x.name).ToArray();

            Application.RequestUserAuthorization(UserAuthorization.WebCam);

            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.LogFormat("Application.RequestUserAuthorization is false");
                return;
            }

            if (m_TargetDevice != string.Empty)
            {
                m_Webcamtex = new WebCamTexture(m_TargetDevice, m_Width, m_Hight);

                Vector3 scale = transform.localScale;

                scale.x = m_Width / m_Resolution;
                scale.y = m_Hight / m_Resolution;

                transform.localScale = scale;
            }
            else
            {
                m_Webcamtex = new WebCamTexture();
            }

            if (m_Webcamtex == null)
            {
                enabled = false;
                return;
            }

            Renderer _renderer = GetComponent<Renderer>();

            if (_renderer == null)
            {
                enabled = false;
                return;
            }

            _renderer.material.mainTexture = m_Webcamtex;

            m_Webcamtex.Play();

            if (Camera.allCamerasCount > 0)
            {
                m_Camera = Camera.allCameras[0];
            }

            if (m_Camera != null && m_Camera.tag != "MainCamera")
            {
                m_Camera.fieldOfView = m_verticalFOV;
                m_Camera.aspect = m_Width / m_Hight;
            }
        }

        private void OnDisable()
        {
            if (m_Webcamtex != null)
            {
                m_Webcamtex.Stop();
            }
        }
    }
}
