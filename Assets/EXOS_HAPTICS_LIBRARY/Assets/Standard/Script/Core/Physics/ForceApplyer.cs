using exiii.Unity.Rx.Triggers;
using System;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity
{
    public class ForceApplyer : MonoBehaviour
    {
        [SerializeField]
        private Collider m_Collider;

        [SerializeField]
        private float m_Strength;

        [SerializeField, Unchangeable]
        private Vector3 m_Vector3;

        [SerializeField, Unchangeable]
        private float m_Distance;

        private void Start()
        {
            m_Collider.OnTriggerStayAsObservable().Subscribe(ApplyForce);
        }

        private void ApplyForce(Collider other)
        {
            if (other.attachedRigidbody == null) { return; }

            var result = Physics.ComputePenetration
                (m_Collider, m_Collider.transform.position, m_Collider.transform.rotation,
                other, other.transform.position, other.transform.rotation,
                out m_Vector3, out m_Distance);

            if (result)
            {
                var point = other.ClosestPoint(m_Collider.transform.position);

                other.attachedRigidbody
                    .AddForceAtPosition(-m_Vector3 * m_Strength, point, ForceMode.Acceleration);
            }
        }
    }
}