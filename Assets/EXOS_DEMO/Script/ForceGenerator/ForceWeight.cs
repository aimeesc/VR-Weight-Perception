using exiii.Unity.EXOS;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class ForceWeight : InteractableNode, IGrabbableScript, IGrabForceGenerator, IGrabPositionGenerator, IGripForceGenerator
    {
        const float Defaultmass = 1.0f;

        #region Inspector

        [Header("Inertia")]
        [SerializeField]
        private bool m_EnableInertia = false;

        [SerializeField]
        private float m_InertiaGain = 0.3f;

        [Header("OutputForce")]
        [SerializeField]
        private float m_ForceMaxDistance = 0.01f;

        [SerializeField]
        private float m_WeightGain = 1f;

        [SerializeField, Unchangeable]
        private Vector3 m_ForceRatioVector = Vector3.zero;

        [Header("SnapPosition")]
        [SerializeField]
        private float m_PositionLimitRatio = 0.03f;

        [SerializeField, Unchangeable]
        private Vector3 m_PositionRatioVector = Vector3.zero;

        [SerializeField]
        private Collider m_Collider;

        #endregion Inspector

        private EnvelopGenerator m_Envelop = new EnvelopGenerator(0f, 1000f, 1000f, 1f, 0.3f);

        private List<Collider> m_IgnoreColliders = new List<Collider>();

        private List<IExTag> m_IgnoreExTags = new List<IExTag>();

        protected override void Awake()
        {
            base.Awake();

            m_IgnoreColliders.Add(m_Collider);

            m_IgnoreExTags.Add(new ExTag<ETag>(ETag.Tools));
        }

        private float Mass
        {
            get
            {
                if (InteractableRoot.Rigidbody != null) { return InteractableRoot.Rigidbody.mass; }

                return Defaultmass;
            }
        }

        private Vector3 Weight
        {
            get { return Vector3.down * Mass * m_WeightGain; }
        }

        public void OnStart(IGrabManipulation manipulation)
        {
            m_PositionRatioVector = Vector3.zero;

            m_Envelop.Start();
        }

        public void OnUpdate(IGrabManipulation manipulation)
        {
            
        }

        public void OnFixedUpdate(IGrabManipulation manipulation)
        {
            
        }

        public void OnEnd(IGrabManipulation manipulation)
        {
            m_PositionRatioVector = Vector3.zero;
        }

        private void OnForceGenerate(IForceReceiver receiver)
        {
            float moveLimit = Time.fixedDeltaTime;

            Vector3 positionRatioVector;

            Vector3 inertia = Vector3.zero;

            if (m_EnableInertia)
            {
                inertia = -receiver.TransformState.Accele * Mass * m_InertiaGain;
            }

            m_ForceRatioVector = Weight * m_Envelop.Value;

            if (m_Collider != null)
            {
                float distance;
                float outputRatio = 1;

                if (ExPhysics.PrimitiveCast(m_Collider, Vector3.down, Mass * m_PositionLimitRatio, out distance, m_IgnoreColliders, m_IgnoreExTags))
                {
                    outputRatio = Mathf.Clamp01(distance / m_ForceMaxDistance);
                }

                m_ForceRatioVector = m_ForceRatioVector * outputRatio + inertia;

                positionRatioVector = Vector3.down * distance + inertia * m_PositionLimitRatio;
            }
            else
            {
                m_ForceRatioVector = m_ForceRatioVector + inertia;

                positionRatioVector = (Weight + inertia) * m_PositionLimitRatio;
            }

            receiver.AddForceRatio(transform.position, m_ForceRatioVector);
           // Debug.Log("this is the force from exos" + m_ForceRatioVector);
            m_PositionRatioVector = Vector3.MoveTowards(m_PositionRatioVector, positionRatioVector, moveLimit);

            //Debug.DrawLine(transform.position, transform.position + m_ForceRatioVector);
        }

        public void OnGenerate(IForceReceiver receiver, IGrabState state)
        {
            OnForceGenerate(receiver);
        }

        public void OnGenerate(IPositionReceiver receiver, IGrabState state)
        {
            receiver.AddPositionRatio(m_PositionRatioVector);
        }

        public void OnGenerate(IForceReceiver receiver, IGripState state)
        {
            OnForceGenerate(receiver);
        }
    }
}