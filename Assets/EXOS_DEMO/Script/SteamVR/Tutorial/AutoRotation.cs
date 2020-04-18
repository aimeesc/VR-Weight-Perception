using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class AutoRotation : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_Rotation;

        // Update is called once per frame
        void Update()
        {
            transform.rotation *= Quaternion.Euler(m_Rotation);
        }
    }
}