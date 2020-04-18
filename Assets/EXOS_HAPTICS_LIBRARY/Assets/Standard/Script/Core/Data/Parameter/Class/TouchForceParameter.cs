using System;
using UnityEngine;

namespace exiii.Unity
{
    [CreateAssetMenu(fileName = "TouchForceParameter", menuName = "EXOS/Parameter/TouchForce")]
    public class TouchForceParameter : ParameterAsset<TouchForceParameter>
    {
        #region Inspector

        [Header(nameof(TouchForceParameter))]
        [SerializeField]
        private float m_ForceMaxDistance = 0.01f;

        public float ForceMaxDistance => m_ForceMaxDistance;

        [SerializeField]
        private float m_ForceGamma = 1.0f;

        public float ForceGamma => m_ForceGamma;

        [SerializeField]
        private float m_FillterGain = 1.0f;

        public float FillterGain => m_FillterGain;

        [SerializeField]
        private float m_RigidForceGain = 1.0f;

        public float RigidForceGain => m_RigidForceGain;

        #endregion Inspector

        protected override void Reset()
        {
            base.Reset();

            m_ForceMaxDistance = 0.01f;
            m_ForceGamma = 1.0f;
            m_FillterGain = 1.0f;
            m_RigidForceGain = 1.0f;
        }

        public override TouchForceParameter CreateCopy(UnityEngine.Object owner)
        {
            var instance = Instantiate(this);

            instance.Owner = owner;
            instance.IsOriginal = false;

            instance.m_ForceMaxDistance = this.m_ForceMaxDistance;
            instance.m_ForceGamma = this.m_ForceGamma;
            instance.m_FillterGain = this.m_FillterGain;
            instance.m_RigidForceGain = this.m_RigidForceGain;

            return instance;
        }
    }
}