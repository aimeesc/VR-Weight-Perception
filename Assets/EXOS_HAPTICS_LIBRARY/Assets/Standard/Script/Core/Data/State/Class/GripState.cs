using UnityEngine;

namespace exiii.Unity
{
    public class GripState : IGripState, ISizeState
    {
        private float m_MaxDistance;

        private bool m_ExpansionRestriction;

        public ISizeState SizeState => this;

        public Transform Center { get; }

        public float Distance { get; set; }

        public float SizeRatio
        {
            get
            {
                if (m_ExpansionRestriction)
                {
                    if (Distance / m_MaxDistance > 1) { m_MaxDistance = Distance; }
                }

                return Distance / m_MaxDistance;
            }
        }

        public GripState(Transform center, float distance, bool expansionRestriction = true)
        {
            Center = center;
            m_MaxDistance = distance;
            m_ExpansionRestriction = expansionRestriction;
        }
    }
}