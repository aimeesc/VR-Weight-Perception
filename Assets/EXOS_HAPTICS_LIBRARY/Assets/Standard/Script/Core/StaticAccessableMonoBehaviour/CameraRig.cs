using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public class CameraRig : StaticAccessableMonoBehaviour<CameraRig>
    {
        [SerializeField, Unchangeable]
        [FormerlySerializedAs("Camera")]
        private Camera m_Camera = null;

        [SerializeField]
        [FormerlySerializedAs("ToolsParentName")]
        private string m_ToolsParentName = "[Tools]";

        [SerializeField, Unchangeable]
        [FormerlySerializedAs("ToolsParent")]
        private Transform m_ToolsParent = null;

        // get camera.
        public static Camera GetCamera()
        {
            if (!IsExist) { return null; }

            if (Instance.m_Camera == null)
            {
                Instance.m_Camera = Instance.GetComponentInChildren<Camera>();
            }

            return Instance.m_Camera;
        }

        // set god object as world.
        public static void SetTools(GameObject toolsObject)
        {
            if (!IsExist) {
                toolsObject.transform.parent = null;
                return;
            }

            if (Instance.m_ToolsParent == null)
            {
                // create parent.
                var parent = new GameObject(Instance.m_ToolsParentName);
                parent.transform.parent = Instance.transform;
                parent.transform.localPosition = Vector3.zero;
                parent.transform.localRotation = Quaternion.identity;

                Instance.m_ToolsParent = parent.transform;
            }

            toolsObject.transform.parent = Instance.m_ToolsParent;
        }
    }
}