using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class HardnessOfTama : MonoBehaviour, IHardness
    {
        public float Hardness { get { return m_Hardness; } }

        [Tooltip("Hard <-> Soft")]
        [SerializeField, Range(0.1f, 0.3f)]
        private float m_Hardness;

    }
}
