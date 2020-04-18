using UnityEngine;
using System;

namespace exiii.Unity
{
	public class FollowCamera : StaticAccessableMonoBehaviour<FollowCamera> 
	{
        [SerializeField]
        private int m_CameraIndex = 0;

        private Vector3 m_Offset = new Vector3(-0.05f, 0, 0);
        private Transform m_Camera;

        protected override void Start()
        {
            base.Start();

            if (Instance != this) { Destroy(gameObject); }

            if (m_CameraIndex >= Camera.allCamerasCount)
            {
                m_CameraIndex = Camera.allCamerasCount - 1;
            }
            else if (m_CameraIndex < 0)
            {
                m_CameraIndex = 0;
            }

            var camera = Camera.allCameras[m_CameraIndex];

            if (camera != null)
            {
                transform.parent = Camera.allCameras[m_CameraIndex].transform;

                transform.ResetLocalState();

                if (camera.tag == "MainCamera")
                {
                    transform.position += m_Offset;
                }
            }
            else
            {
                enabled = false;
            }
        }
    }
}
