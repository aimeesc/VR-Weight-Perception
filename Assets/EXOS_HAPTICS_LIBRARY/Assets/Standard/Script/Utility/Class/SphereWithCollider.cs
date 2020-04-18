using UnityEngine;

namespace exiii.Unity
{
    public class SphereWithCollider : ISphere
    {
        private SphereCollider m_SphereCollider;

        public Vector3 Center => m_SphereCollider.transform.TransformPoint(m_SphereCollider.center);

        public float Radius => m_SphereCollider.radius;

        public SphereWithCollider(SphereCollider collider)
        {
            m_SphereCollider = collider;
        }
    }
}