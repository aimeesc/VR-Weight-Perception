using UnityEngine;
using UnityEngine.XR;

namespace exiii.Unity.XR
{
    public class XRTracking : MonoBehaviour
    {
        [SerializeField]
        private XRNode m_XRNode;

        void Update()
        {
            transform.localPosition = InputTracking.GetLocalPosition(m_XRNode);
            transform.localRotation = InputTracking.GetLocalRotation(m_XRNode);
        }
    }
}