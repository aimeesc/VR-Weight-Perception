using UnityEngine;

namespace exiii.Unity
{
    public class SphereWithTransform : ISphere
    {
        private Transform m_Transform;

        public Vector3 Center => m_Transform.position;

        public float Radius { get; set; }

        public SphereWithTransform(Transform transform, float radius)
        {
            m_Transform = transform;
            Radius = radius;
        }
    }
}