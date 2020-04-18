using exiii.Unity.EXOS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class BitCollidingForceGenerator : MonoBehaviour, IGrabForceGenerator
    {
        [SerializeField]
        private Transform m_Target;
        [SerializeField]
        private float m_Gain;
        [SerializeField]
        private Vector3 m_Force;

        private bool m_IsScrew = false;

        private void OnTriggerStay(Collider other)
        {
            m_Force = Vector3.zero;
            if (other.gameObject.tag == "Screw")
            {
                m_IsScrew = true;
                Vector3 duration = m_Target.position - transform.position;
                m_Force = duration.normalized * duration.magnitude * m_Gain;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            m_IsScrew = false;
        }

        private void OnForceGenerate(IForceReceiver receiver)
        {
            receiver.AddForceRatio(transform.position, m_Force);
        }

        public void OnGenerate(IForceReceiver receiver, IGrabState state)
        {
            if (m_IsScrew)
            {
                OnForceGenerate(receiver);
            }
        }

    }

}
