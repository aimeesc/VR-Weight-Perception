using exiii.Unity.Rx.Triggers;
using System;
using UnityEngine;

namespace exiii.Unity
{
    public class TransformState : IHasTransform
    {
        public Transform transform { get; private set; }

        private const float velocityGain = 1.0f;
        private const float acceleGain = 0.1f;
        private const float mergin = 0.01f;

        private Vector3 filteredVelocity, filteredAngulerVelocity, filteredAccele, filteredAngulerAccele;

        private Vector3 m_velocity, m_angulerVelocity, m_accele, m_angulerAccele;
        private Vector3 m_prePositon;
        private Quaternion m_preRotation;

        private Vector3Filter m_fillterVelocity, m_fillterAngulerVelocity, m_fillterAccele, m_fillterAngulerAccele;

        public Vector3 Velocity
        {
            get { return filteredVelocity; }
        }

        public Vector3 AngulerVelocityAsDigree
        {
            get { return filteredAngulerVelocity / (Mathf.PI / 180f); }
        }

        public Vector3 AngulerVelocity
        {
            get { return filteredAngulerVelocity; }
        }

        public Vector3 Accele
        {
            get { return filteredAccele; }
        }

        public Vector3 AngulerAccele
        {
            get { return filteredAngulerAccele; }
        }

        public TransformState(Transform transform)
        {
            this.transform = transform;

            m_fillterVelocity = new Vector3Filter(0.70f);
            m_fillterAngulerVelocity = new Vector3Filter(0.70f);

            m_fillterAccele = new Vector3Filter(0.95f);
            m_fillterAngulerAccele = new Vector3Filter(0.95f);

            transform.FixedUpdateAsObservable().Subscribe(_ => FixedUpdate());
        }

        protected virtual void FixedUpdate()
        {
            m_velocity = (transform.position - m_prePositon) / Time.fixedDeltaTime;

            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(m_preRotation);

            float theta = 2.0f * Mathf.Acos(Mathf.Clamp(deltaRotation.w, -1.0f, 1.0f));

            if (theta > Mathf.PI) { theta -= 2.0f * Mathf.PI; }

            m_angulerVelocity = new Vector3(deltaRotation.x, deltaRotation.y, deltaRotation.z);

            if (m_angulerVelocity.sqrMagnitude > 0.0f)
            {
                m_angulerVelocity = theta * m_angulerVelocity.normalized / Time.fixedDeltaTime;
            }

            m_accele = (m_velocity - m_fillterVelocity.LastOutput) / Time.fixedDeltaTime;
            m_angulerAccele = (m_angulerVelocity - m_fillterAngulerVelocity.LastOutput) / Time.fixedDeltaTime;

            m_prePositon = transform.position;
            m_preRotation = transform.rotation;

            filteredVelocity = m_fillterVelocity.Input(m_velocity) * velocityGain;
            filteredAngulerVelocity = m_fillterAngulerVelocity.Input(m_angulerVelocity) * velocityGain;

            filteredAccele = m_fillterAccele.Input(m_accele) * acceleGain;
            filteredAngulerAccele = m_fillterAngulerAccele.Input(m_angulerAccele) * acceleGain;

            /*
            if (filteredVelocity.magnitude < mergin) { filteredVelocity = Vector3.zero; }
            if (filteredAngulerVelocity.magnitude < mergin) { filteredAngulerVelocity = Vector3.zero; }
            if (filteredAccele.magnitude < mergin) { filteredAccele = Vector3.zero; }
            if (filteredAngulerAccele.magnitude < mergin) { filteredAngulerAccele = Vector3.zero; }
            */
        }
    }
}