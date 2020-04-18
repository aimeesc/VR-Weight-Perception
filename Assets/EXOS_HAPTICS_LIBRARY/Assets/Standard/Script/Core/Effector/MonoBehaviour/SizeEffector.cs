using UnityEngine;

namespace exiii.Unity
{
    public class SizeEffector : InteractableNode, ISizeEffector
    {
        [SerializeField]
        private float m_DeformThreashold = 0.9f;

        private Vector3 m_InitioalScale;

        protected override void Awake()
        {
            base.Awake();

            m_InitioalScale = transform.localScale;
        }

        public void OnChangeSizeRatio(ISizeState state)
        {
            if (InteractableRoot.PhysicalProperties.Elasticity > m_DeformThreashold) { return; }

            transform.localScale = m_InitioalScale * state.SizeRatio;
        }

        public void OnResetSizeRatio()
        {
            transform.localScale = m_InitioalScale;
        }
    }
}