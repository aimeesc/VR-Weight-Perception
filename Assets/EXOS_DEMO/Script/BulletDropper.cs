using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.Sample
{
    // set gravity if velocity is lower.
    public class BulletDropper : MonoBehaviour
    {

        [SerializeField]
        [FormerlySerializedAs("Threshold")]
        private float m_Threshold = 0.0f;

        private Rigidbody m_Rigidbody = null;

        // Use this for initialization
        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {

            // drop.
            Drop();
        }

        // drop.
        private void Drop()
        {
            if (m_Rigidbody == null || m_Rigidbody.useGravity) { return; }

            if (m_Rigidbody.velocity.magnitude < m_Threshold)
            {
                m_Rigidbody.useGravity = true;
            }
        }
    }
}