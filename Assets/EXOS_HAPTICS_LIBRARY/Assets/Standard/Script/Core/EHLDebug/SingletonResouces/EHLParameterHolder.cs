using UnityEngine;
using System.Collections;

namespace exiii.Unity
{
    // For Experiment in develop
    [CreateAssetMenu(fileName = "EHL", menuName = "EXOS/Editor/Parameter/EHL")]
    public class EHLParameterHolder : StaticAccessableScriptableObject<EHLParameterHolder>
    {
        #region Static

        private static bool UseCopiedParameter { get { return Instance.m_UseCopiedParameter; } }

        public static PhysicsParameter PhysicsParameter
        {
            get
            {
                if (UseCopiedParameter)
                {
                    return Instance.m_PhysicsParameter.CreateCopy(Instance);
                }
                else
                {
                    return Instance.m_PhysicsParameter;
                }                
            }
        }

        public static FollowTargetParameter FollowTargetParameter
        {
            get
            {
                if (UseCopiedParameter)
                {
                    return Instance.m_FollowTargetParameter.CreateCopy(Instance);
                }
                else
                {
                    return Instance.m_FollowTargetParameter;
                }
            }
        }

        public static bool UseBrake { get { return Instance.m_UseBrake; } }

        public static float MoveLimit { get { return Instance.m_MoveLimit; } }

        public static float RotateLimit { get { return Instance.m_RotateLimit; } }


        public static bool UseSpring { get { return Instance.m_UseSpring; } }

        public static ESpringType SpringType { get { return Instance.m_SpringType; } }

        public static float SpringForce { get { return Instance.m_SpringForce; } }

        public static float SpringPower { get { return Instance.m_SpringPower; } }

        public static bool DrawLine { get { return Instance.m_DrawLine; } }


        public static bool UseJoint { get { return Instance.m_UseJoint; } }

        #endregion

        #region Inspector

        [Header("Parameter")]
        [SerializeField]
        private bool m_UseCopiedParameter = true;

        [SerializeField]
        private PhysicsParameter m_PhysicsParameter;

        [SerializeField]
        private FollowTargetParameter m_FollowTargetParameter;

        [Header("Brake")]
        [SerializeField]
        private bool m_UseBrake = false;

        [SerializeField]
        private float m_MoveLimit = 0.1f;

        [SerializeField]
        private float m_RotateLimit = 0.1f;

        [Header("Spring")]
        [SerializeField, UnchangeableInPlaying]
        private bool m_UseSpring = false;

        [SerializeField, UnchangeableInPlaying]
        private ESpringType m_SpringType;

        [SerializeField]
        private float m_SpringForce = 1000f;

        [SerializeField]
        private float m_SpringPower = 1.0f;

        [SerializeField]
        private bool m_DrawLine = false;

        [Header("Spring")]
        [SerializeField]
        private bool m_UseJoint = false;

        #endregion Inspector
    }
}
