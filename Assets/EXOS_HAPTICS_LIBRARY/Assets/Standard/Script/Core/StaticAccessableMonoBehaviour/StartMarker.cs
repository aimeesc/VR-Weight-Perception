using UnityEngine;

namespace exiii.Unity
{
    public class StartMarker : StaticAccessableMonoBehaviour<StartMarker>
    {
#pragma warning disable 414

        [Header("Gizmos")]
        [SerializeField]
        private Mesh m_GizmosMesh;

        [SerializeField]
        private Color m_GizmosColor = Color.cyan;

        [SerializeField]
        private Vector3 m_Scale = Vector3.one;

#pragma warning restore 414

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (m_GizmosMesh != null && transform != null)
            {
                Color original = Gizmos.color;
                Gizmos.color = m_GizmosColor;

                Gizmos.DrawMesh(m_GizmosMesh, transform.position, transform.rotation, m_Scale);

                Gizmos.color = original;
            }
        }

#endif
    }
}