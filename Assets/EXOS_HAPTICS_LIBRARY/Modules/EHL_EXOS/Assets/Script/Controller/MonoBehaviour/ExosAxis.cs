using exiii.Extensions;
using exiii.Unity.Device;
using System;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity.EXOS
{
    [DisallowMultipleComponent]
    public class ExosAxis : MonoBehaviour
    {
        #region Inspector

        [SerializeField]
        private EAxisType m_AxisType = EAxisType.Pinch;

        public EAxisType AxisType
        {
            get { return m_AxisType; }
        }

        [SerializeField]
        private Vector3 m_LocalAxis = Vector3.zero;

        public Vector3 LocalAxis
        {
            get { return m_LocalAxis; }
        }

        [SerializeField]
        private AxisParameter m_AxisParameter = new AxisParameter();

        [Header("Debug")]
        [SerializeField, Unchangeable]
        private float m_AngleRatio;

        public float AngleRatio { get { return m_AngleRatio; } set { m_AngleRatio = value; } }

        [SerializeField]
        private Vector3 m_InitialLotationVector;

        #endregion Inspector

        private Quaternion m_InitialLotation;

        public string AxisName
        {
            get { return m_AxisType.EnumToString(); }
        }

        public Vector3 WorldAxis
        {
            get { return transform.TransformVector(m_LocalAxis); }
        }

        public Vector2 AngleLimit
        {
            get { return m_AxisParameter.AngleLimit; }
        }

        private void Awake()
        {
            m_InitialLotation = transform.localRotation;

            m_InitialLotationVector = m_InitialLotation.eulerAngles;
        }

        private void OnDrawGizmos()
        {
            if (m_AxisParameter.DrawGizmos == false) { return; }

            Gizmos.DrawLine(transform.position, transform.position + WorldAxis * m_AxisParameter.GizmosLength);
        }

        public Vector3 GetClosestSegmentToAxis(Vector3 point)
        {
            return Vector3.ProjectOnPlane(point - transform.position, WorldAxis);
        }
    }

    [Serializable]
    public class AxisParameter
    {
        #region Inspector

        [Header(nameof(AxisParameter))]
        [SerializeField]
        private Vector2 m_AngleLimit = Vector2.zero;

        public Vector2 AngleLimit => m_AngleLimit;

        [SerializeField]
        private bool m_DrawGizmos = false;

        public bool DrawGizmos => m_DrawGizmos;

        [SerializeField]
        private float m_GizmosLength = 0.1f;

        public float GizmosLength => m_GizmosLength;

        #endregion Inspector

        public AxisParameter()
        {
            this.m_AngleLimit = Vector2.zero;
            this.m_DrawGizmos = false;
            this.m_GizmosLength = 0.1f;
        }

        public AxisParameter(AxisParameter parameter)
        {
            this.m_AngleLimit = parameter.AngleLimit;
            this.m_DrawGizmos = parameter.DrawGizmos;
            this.m_GizmosLength = parameter.GizmosLength;
        }
    }
}

