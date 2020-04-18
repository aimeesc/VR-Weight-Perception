using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    /// Implementation of interface to calculate speed and acceleration from transform change
    /// </summary>
    public class TransformPhysics : MonoBehaviour
    {
        [Header("TransformPhysics")]
        [SerializeField]
        protected float velocityGain = 1.0f;

        [SerializeField]
        protected float acceleGain = 0.1f;

        [SerializeField]
        protected float mergin = 0.01f;

        protected Vector3 filteredVelocity, filteredAngulerVelocity, filteredAccele, filteredAngulerAccele;

        public Vector3 velocity
        {
            get { return filteredVelocity; }
        }

        public Vector3 angulerVelocity
        {
            get { return filteredAngulerVelocity; }
        }

        public Vector3 accele
        {
            get { return filteredAccele; }
        }

        public Vector3 angulerAccele
        {
            get { return filteredAngulerAccele; }
        }

        private Vector3 m_velocity, m_angulerVelocity, m_accele, m_angulerAccele;
        private Vector3 m_prePositon, m_preRotation;

        private Vector3Filter m_fillterVelocity, m_fillterAngulerVelocity, m_fillterAccele, m_fillterAngulerAccele;

        protected virtual void Awake()
        {
            m_fillterVelocity = new Vector3Filter(0.70f);
            m_fillterAngulerVelocity = new Vector3Filter(0.70f);

            m_fillterAccele = new Vector3Filter(0.95f);
            m_fillterAngulerAccele = new Vector3Filter(0.95f);
        }

        protected virtual void FixedUpdate()
        {
            m_velocity = (transform.position - m_prePositon) / Time.fixedDeltaTime;
            m_angulerVelocity = (transform.eulerAngles - m_preRotation) / Time.fixedDeltaTime;

            m_accele = (m_velocity - m_fillterVelocity.LastOutput) / Time.fixedDeltaTime;
            m_angulerAccele = (m_angulerVelocity - m_fillterAngulerVelocity.LastOutput) / Time.fixedDeltaTime;

            m_prePositon = transform.position;
            m_preRotation = transform.eulerAngles;

            filteredVelocity = m_fillterVelocity.Input(m_velocity) * velocityGain;
            filteredAngulerVelocity = m_fillterAngulerVelocity.Input(m_angulerVelocity) * velocityGain;

            filteredAccele = m_fillterAccele.Input(m_accele) * acceleGain;
            filteredAngulerAccele = m_fillterAngulerAccele.Input(m_angulerAccele) * acceleGain;

            if (filteredVelocity.magnitude < mergin) { filteredVelocity = Vector3.zero; }
            if (filteredAngulerVelocity.magnitude < mergin) { filteredAngulerVelocity = Vector3.zero; }
            if (filteredAccele.magnitude < mergin) { filteredAccele = Vector3.zero; }
            if (filteredAngulerAccele.magnitude < mergin) { filteredAngulerAccele = Vector3.zero; }
        }
    }
}