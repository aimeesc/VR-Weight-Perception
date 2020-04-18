using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace exiii.Unity.Develop
{
    public class SceneCameraCenter : StaticAccessableMonoBehaviour<SceneCameraCenter>
    {
        public float Size = 0.1f;

        [Unchangeable]
        public float CameraDistance;

        void OnDrawGizmos()
        {
            //Transform CameraTransform = SceneView.lastActiveSceneView.camera.transform;
            //Vector3 ViewPoint = CameraTransform.position + CameraTransform.forward * SceneView.lastActiveSceneView.cameraDistance;

            CameraDistance = SceneView.lastActiveSceneView.cameraDistance;

            Gizmos.DrawWireSphere(SceneView.lastActiveSceneView.pivot, Size);
        }
    }
}