using exiii.Extensions;
using UnityEngine;

namespace exiii.Unity
{
    public class SwitchEnable : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] m_TargetGameObject;

        [SerializeField]
        private MonoBehaviour[] m_TargetMonoBehaviour;

        [SerializeField]
        private Renderer[] m_TargetRenderer;

        public void SwitchGameObject()
        {
            if (m_TargetGameObject == null) { return; }

            m_TargetGameObject.Foreach(x => x.gameObject.SetActive(!x.gameObject.activeSelf));
        }

        public void SwitchMonoBehaviour()
        {
            if (m_TargetMonoBehaviour == null) { return; }

            m_TargetMonoBehaviour.Foreach(x => x.enabled = !x.enabled);
        }

        public void SwitchRenderer()
        {
            if (m_TargetRenderer == null) { return; }

            m_TargetRenderer.Foreach(x => x.enabled = !x.enabled);
        }
    }
}