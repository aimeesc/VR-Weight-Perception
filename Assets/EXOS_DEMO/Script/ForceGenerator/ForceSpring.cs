using exiii.Unity.EXOS;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class ForceSpring : InteractableNode, IGrabForceGenerator, IGripForceGenerator
    {
        [SerializeField]
        private Transform m_Target;

        [SerializeField]
        private float m_Length;

        [SerializeField]
        private float m_Gain = 1.0f;

        [SerializeField, Unchangeable, Graph(60)]
        private Vector3 m_Force;

        protected override void Start()
        {
            if (m_Target == null)
            {
                Debug.Log($"Target is null");
                gameObject.SetActive(false);
            }
        }

        private void OnForceGenerate(IForceReceiver receiver)
        {
            Vector3 duration = m_Target.position - transform.position;

            if (duration.magnitude > m_Length)
            {
                m_Force = duration.normalized * (duration.magnitude - m_Length) * m_Gain;
                receiver.AddForceRatio(transform.position, m_Force);
            }
        }

        public void OnGenerate(IForceReceiver receiver, IGrabState state)
        {
            OnForceGenerate(receiver);
        }

        public void OnGenerate(IForceReceiver receiver, IGripState state)
        {
            OnForceGenerate(receiver);
        }
    }
}