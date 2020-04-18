using exiii.Extensions;
using System;
using UnityEngine;

namespace exiii.Unity.EXOS
{
    /// <summary>
    ///
    /// </summary>
    public class ExosHinge
    {
        private const float startRatio = 0.5f;

        private ExosAxis[] m_Axises;

        private float m_AngleMin;
        private float m_AngleMax;

        private Quaternion m_InitialAngle;

        /// <summary>
        /// Acquire the rotation angle around the axis
        /// </summary>
        public float Angle
        {
            get { return m_AngleMin + (m_AngleMax - m_AngleMin) * m_AngleRatio; }
        }

        private float m_AngleRatio;

        /// <summary>
        /// Set and obtain the rotation angle around the axis in the range from 0 to 1
        /// </summary>
        public float AngleRatio
        {
            get { return m_AngleRatio; }

            set
            {
                m_AngleRatio = value;

                m_Axises.Foreach(axis => axis.AngleRatio = m_AngleRatio);

                m_Axises.Foreach(axis => axis.transform.localRotation = m_InitialAngle * CalcAngle(axis));
            }
        }

        private Quaternion CalcAngle(ExosAxis axis)
        {
            var angle = Quaternion.AngleAxis(Angle, axis.LocalAxis);

            if (angle.IsNaN())
            {
                return Quaternion.identity;
            }
            else
            {
                return angle;
            }
        }

        public ExosHinge(ExosAxis[] axises, ExosAxis reference)
        {
            if (axises.Length == 0) { throw new InvalidOperationException("ExosHinge axis is not found"); }

            if (reference == null)
            {
                reference = axises[0];
                Debug.LogWarning($"refernce axis is null");
            }

            m_Axises = axises;

            m_InitialAngle = reference.transform.localRotation;

            m_AngleMin = reference.AngleLimit.x;
            m_AngleMax = reference.AngleLimit.y;

            AngleRatio = startRatio;
        }
    }
}

