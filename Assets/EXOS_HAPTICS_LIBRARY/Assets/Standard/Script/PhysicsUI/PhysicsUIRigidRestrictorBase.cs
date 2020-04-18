using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.PhysicsUI
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsUIRigidRestrictorBase : ExMonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("LocalFleezeX")]
        protected bool m_LocalFleezeX = true;

        [SerializeField, FormerlySerializedAs("LocalFleezeY")]
        protected bool m_LocalFleezeY = false;

        [SerializeField, FormerlySerializedAs("LocalFleezeZ")]
        protected bool m_LocalFleezeZ = true;

        [SerializeField, FormerlySerializedAs("AutoDetection")]
        protected bool m_AutoDetection = true;

        [SerializeField, FormerlySerializedAs("LocalRangeX")]
        protected Vector2 m_LocalRangeX = Vector2.zero;

        [SerializeField, FormerlySerializedAs("LocalRangeY")]
        protected Vector2 m_LocalRangeY = Vector2.zero;

        [SerializeField, FormerlySerializedAs("LocalRangeZ")]
        protected Vector2 m_LocalRangeZ = Vector2.zero;

        [SerializeField, FormerlySerializedAs("AttenuateVelocityX")]
        protected bool m_AttenuateVelocityX = false;

        [SerializeField, FormerlySerializedAs("AttenuateVelocityY")]
        protected bool m_AttenuateVelocityY = false;

        [SerializeField, FormerlySerializedAs("AttenuateVelocityZ")]
        protected bool m_AttenuateVelocityZ = false;

        protected Rigidbody m_RigidBody = null;
        protected Vector3 m_DefaultPosition = Vector3.zero;

        // on Awake.
        protected override void Awake()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_DefaultPosition = transform.localPosition;

            base.Awake();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // restrict velocity.
            RestrictVelocity();

            // restrict position.
            RestrictPosition();

            // attenuate velocity.
            AttenuateVelocity();
        }

        // reset UI state.
        public void ResetUIDefault()
        {
            transform.localPosition = m_DefaultPosition;
            m_RigidBody.velocity = Vector3.zero;
        }

        // reset UI max.
        public void ResetUIMax()
        {
            Vector3 target = Vector3.zero;
            target.x = m_LocalRangeX.y;
            target.y = m_LocalRangeY.y;
            target.z = m_LocalRangeZ.y;
            transform.localPosition = target;
            m_RigidBody.velocity = Vector3.zero;
        }

        // reset UI max.
        public void ResetUIMin()
        {
            Vector3 target = Vector3.zero;
            target.x = m_LocalRangeX.x;
            target.y = m_LocalRangeY.x;
            target.z = m_LocalRangeZ.x;
            transform.localPosition = target;
            m_RigidBody.velocity = Vector3.zero;
        }

        // restrict velocity.
        void RestrictVelocity()
        {
            // fleeze velocity.
            Vector3 localVelocity = transform.InverseTransformDirection(m_RigidBody.velocity);
            localVelocity.x = (m_LocalFleezeX) ? 0.0f : localVelocity.x;
            localVelocity.y = (m_LocalFleezeY) ? 0.0f : localVelocity.y;
            localVelocity.z = (m_LocalFleezeZ) ? 0.0f : localVelocity.z;
            m_RigidBody.velocity = transform.TransformDirection(localVelocity);
        }

        // attenuate velocity.
        protected void AttenuateVelocity()
        {
            if (!m_AttenuateVelocityX && !m_AttenuateVelocityY && !m_AttenuateVelocityZ) { return; }

            Vector3 localVelocity = transform.InverseTransformDirection(m_RigidBody.velocity);
            localVelocity.x = (m_AttenuateVelocityX) ? localVelocity.x * 0.9f : localVelocity.x;
            localVelocity.y = (m_AttenuateVelocityY) ? localVelocity.y * 0.9f : localVelocity.y;
            localVelocity.z = (m_AttenuateVelocityZ) ? localVelocity.z * 0.9f : localVelocity.z;
            m_RigidBody.velocity = transform.TransformDirection(localVelocity);
        }

        // restrict position.
        void RestrictPosition()
        {
            Vector3 localPosition = transform.localPosition;

            // restrict position in range.
            if (!m_LocalFleezeX)
            {
                if (localPosition.x < m_LocalRangeX.x)
                {
                    ResetUIMin();
                    return;
                }
                else if (localPosition.x > m_LocalRangeX.y)
                {
                    ResetUIMax();
                    return;
                }
            }
            if (!m_LocalFleezeY)
            {
                if (localPosition.y < m_LocalRangeY.x)
                {
                    ResetUIMin();
                    return;
                }
                else if (localPosition.y > m_LocalRangeY.y)
                {
                    ResetUIMax();
                    return;
                }
            }
            if (!m_LocalFleezeZ)
            {
                if (localPosition.z < m_LocalRangeZ.x)
                {
                    ResetUIMin();
                    return;
                }
                else if (localPosition.z > m_LocalRangeZ.y)
                {
                    ResetUIMax();
                    return;
                }
            }

            // fleeze local position.
            localPosition.x = (m_LocalFleezeX) ? m_DefaultPosition.x : localPosition.x;
            localPosition.y = (m_LocalFleezeY) ? m_DefaultPosition.y : localPosition.y;
            localPosition.z = (m_LocalFleezeZ) ? m_DefaultPosition.z : localPosition.z;
            transform.localPosition = localPosition;
        }
    }
}
