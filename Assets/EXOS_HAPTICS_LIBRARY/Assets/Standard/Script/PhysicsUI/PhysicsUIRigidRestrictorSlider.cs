using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.PhysicsUI
{
    public class PhysicsUIRigidRestrictorSlider : PhysicsUIRigidRestrictorBase
    {
        [SerializeField, FormerlySerializedAs("Handle")]
        private Transform m_Handle = null;

        [SerializeField, FormerlySerializedAs("TermL")]
        private Transform m_TermL = null;

        [SerializeField, FormerlySerializedAs("TermR")]
        private Transform m_TermR = null;

        // on awake.
        protected override void Awake()
        {
            // init range x.
            if (m_AutoDetection && !m_LocalFleezeX)
            {
                InitRangeX();
            }

            base.Awake();
        }

        // init range.
        private void InitRangeX()
        {
            // get length from game objects.
            float lengthRange = Mathf.Abs(m_TermR.localPosition.x - m_TermL.localPosition.x);
            float lengthHandle = m_Handle.localScale.x;
            float lengthTerm = m_TermL.localScale.x;

            // calc movable length.
            float movable = lengthRange - lengthTerm - lengthHandle;

            m_LocalRangeX.x = 1.01f * -movable / 2.0f;
            m_LocalRangeX.y = 1.01f * movable / 2.0f;
        }        
    }
}
