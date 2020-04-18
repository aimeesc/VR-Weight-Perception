using UnityEngine;

namespace exiii.Unity
{
    [CreateAssetMenu(fileName = "PhysicalProperties", menuName = "EXOS/PhysicalProperties")]
    public class PhysicalProperties : ScriptableObject
    {
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float elasticity = 1.0f;

        public float Elasticity
        {
            get { return elasticity; }
            set { elasticity = value; }
        }

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float viscosity = 0.0f;

        public float Viscosity
        {
            get { return viscosity; }
            set { viscosity = value; }
        }

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float inertia = 1.0f;

        public float Inertia
        {
            get { return inertia; }
            set { inertia = value; }
        }

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float surfaceHardness = 0.1f;

        public float SurfaceHardness
        {
            get { return surfaceHardness; }
            set { surfaceHardness = value; }
        }

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float friction = 0.0f;

        public float Friction
        {
            get { return friction; }
            set { friction = value; }
        }

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float roughness = 0.0f;

        public float Roughness
        {
            get { return roughness; }
            set { roughness = value; }
        }
    }
}