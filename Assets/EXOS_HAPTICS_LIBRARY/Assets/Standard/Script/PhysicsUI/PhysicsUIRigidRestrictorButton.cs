using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.PhysicsUI
{
    
    [RequireComponent(typeof(SpringJoint))]
    public class PhysicsUIRigidRestrictorButton : PhysicsUIRigidRestrictorBase
    {
        [SerializeField, FormerlySerializedAs("TransformCover")]
        private Transform m_TransformCover = null;

        [Header("PointPanel")]

        [SerializeField]
        [FormerlySerializedAs("Physics")]
        private Transform m_Physics = null;

        [SerializeField]
        [FormerlySerializedAs("DebugTarget")]
        private GameObject m_DebugTarget = null;

        public Vector3 LastClosetPoint { get; set; } = Vector3.zero;

        // on awake.
        protected override void Awake()
        {
            if (m_AutoDetection && !m_LocalFleezeY)
            {
                // detect range min.
                if (m_TransformCover != null)
                {
                    m_LocalRangeY.x = m_TransformCover.localPosition.y * 1.01f;
                }

                // detect range max.
                m_LocalRangeY.y = transform.localPosition.y * 1.01f;
            }

            base.Awake();
        }

        // detect hit position for debug.
        public void OnTriggerEnter(Collider other)
        {
            if (m_Physics == null) { return; }

            // show debug hit pos.
            if (m_DebugTarget != null)
            {
                m_DebugTarget.transform.position = LastClosetPoint;
            }

            // calc closet point;
            LastClosetPoint = other.ClosestPointOnBounds(this.transform.position);
            Vector3 calc = Vector3.zero;
            calc = m_Physics.InverseTransformPoint(LastClosetPoint);
            calc.x *= transform.lossyScale.x;
            calc.y *= transform.lossyScale.y;
            calc.z *= transform.lossyScale.z;
            LastClosetPoint = calc;
        }
    }    
}
