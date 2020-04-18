using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace exiii.Unity
{
	public class CollisionChecker : MonoBehaviour 
	{
        #region Inspector

        [Header(nameof(CollisionChecker))]

        [SerializeField]
        private bool m_Enable = true;

		[SerializeField]
		private Color m_Color;

        private MeshRenderer m_MeshRenderer;

        #endregion Inspector

        private Color m_ColorBuffer;

        private HashSet<Collider> m_Others = new HashSet<Collider>();

        private void Awake()
        {
            if (m_MeshRenderer == null)
            {
                m_MeshRenderer = GetComponent<MeshRenderer>();
            }

            m_ColorBuffer = m_MeshRenderer.material.color;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger || !m_Enable) { return; }

            m_Others.Add(other);

            if (m_Others.Count > 0)
            {
                m_MeshRenderer.material.color = m_Color;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.isTrigger || !m_Enable) { return; }

            m_Others.Remove(other);

            if (m_Others.Count == 0)
            {
                m_MeshRenderer.material.color = m_ColorBuffer;
            }
        }
    }
}
