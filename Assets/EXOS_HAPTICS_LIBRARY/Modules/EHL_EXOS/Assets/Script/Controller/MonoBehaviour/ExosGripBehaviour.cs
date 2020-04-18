using exiii.Unity.Device;
using UnityEngine;

namespace exiii.Unity.EXOS
{
    public class ExosGripBehaviour : GripBehaviourTwoFinger
    {
        #region Inspector

        [Header(nameof(ExosGripBehaviour))]
        [SerializeField]
        private ExosDevice m_ExDevice;

        [SerializeField]
        private float m_ReleaseAngleRatio = 0.95f;

        public float ReleaseAngleRatio => m_ReleaseAngleRatio;

        #endregion Inspector

        private ExosJoint m_JointPinch;

        public override bool AllowGrip
        {
            get
            {
                if (m_JointPinch != null)
                {
                    return m_JointPinch.AngleRatio < ReleaseAngleRatio;
                }
                else
                {
                    return true;
                }
            }
        }

        private void Start()
        {
            if (m_ExDevice == null || !m_ExDevice.TryGetJoint(EAxisType.Pinch, out m_JointPinch))
            {
                EHLDebug.LogWarning("GetJoint failed", this, "Controller", ELogLevel.Overview);
            }
        }
    }
}

