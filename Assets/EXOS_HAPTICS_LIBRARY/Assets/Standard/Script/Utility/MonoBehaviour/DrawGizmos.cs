using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    /// Show Gizmos for debugging
    /// </summary>
    public class DrawGizmos : MonoBehaviour
    {
        public enum GizmosMode
        {
            Sphere,
            WireSphere,
            Cube,
            WireCube,
            Icon,
        }

        [SerializeField]
        private GizmosMode m_Mode;

        [SerializeField]
        private float m_SizeRatio = 1;

        private float m_SphereRadius = 0.01f;
        private Vector3 m_CubeSize = new Vector3(0.01f, 0.01f, 0.01f);

        private void OnDrawGizmos()
        {
            switch (m_Mode)
            {
                case GizmosMode.Sphere:
                    Gizmos.DrawSphere(transform.position, m_SphereRadius * m_SizeRatio);
                    break;

                case GizmosMode.WireSphere:
                    Gizmos.DrawWireSphere(transform.position, m_SphereRadius * m_SizeRatio);
                    break;

                case GizmosMode.Cube:
                    Gizmos.DrawCube(transform.position, m_CubeSize * m_SizeRatio);
                    break;

                case GizmosMode.WireCube:
                    Gizmos.DrawWireCube(transform.position, m_CubeSize * m_SizeRatio);
                    break;

                case GizmosMode.Icon:
                    Gizmos.DrawIcon(transform.position, name);
                    break;
            }
        }
    }
}