using System;
using UnityEngine;

namespace exiii.Unity
{
    public class BonePenetratorHolder : PenetratorHolder
    {
        #region Inspector

        [Header(nameof(BonePenetratorHolder))]

        [SerializeField]
        private bool m_UseCollider = true;

#pragma warning disable 414
        [SerializeField, Unchangeable]
        private float m_Mass;
#pragma warning restore 414

        #endregion Inspector

        public float Radius => m_Penetrator.Sphere.Radius;

        private SpherePenetrator m_Penetrator;

        public override IPenetrator Penetrator { get { return m_Penetrator; } }

        public override void SetValues(float size, float mass)
        {
            if (m_UseCollider)
            {
                AddCollider(size);
            }
            else
            {
                AddSphere(size);
            }

            m_Mass = mass;
        }

        public override void SetVisible(bool visible)
        {
        }

        private void AddCollider(float size)
        {
            var collider = gameObject.GetOrAddComponent<SphereCollider>();

            collider.isTrigger = true;
            collider.radius = size / 2;

            m_Penetrator = new SpherePenetrator(new SphereWithCollider(collider), collider);
        }

        private void AddSphere(float size)
        {
            m_Penetrator = new SpherePenetrator(new SphereWithTransform(transform, size / 2), null);
        }
    }
}