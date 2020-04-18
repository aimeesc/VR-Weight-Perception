using exiii.Extensions;
using exiii.Library.IO;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.Device
{
    /// <summary>
    /// Handle each axis information of EXOS device
    /// </summary>
    [Serializable]
    public class ExosJoint
    {
        #region Inspector

        [Header(nameof(ExosJoint))]

        [SerializeField, Unchangeable]
        private string m_Name;

        [SerializeField]
        private EAxisType m_AxisType = EAxisType.Pinch;

        /// <summary>
        /// Type of axis
        /// </summary>
        public EAxisType AxisType => m_AxisType;

        /// <summary>
        /// Index of target axis
        /// </summary>
        [Header("Angle")]
        [SerializeField, Range(0, 1)]
        [FormerlySerializedAs("AngleIndex")]
        private int m_AngleIndex;

        /// <summary>
        /// Upper and lower limit of angle
        /// </summary>
        [Header("Value as Min/Max")]
        [SerializeField, Unchangeable]
        [FormerlySerializedAs("AngleLimit")]
        private Vector2 m_AngleLimit;

        /// <summary>
        /// Whether to invert the angle read from the device
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("InvertAngle")]
        private bool m_InvertAngle;

        [SerializeField, Unchangeable]
        private int m_Angle;

        [SerializeField, Unchangeable]
        private float m_AngleRatio;

        [SerializeField]
        private float m_AngleRatioDefault = 0.0f;

        /// <summary>
        /// Index of target actuator
        /// </summary>
        [Header("Force")]
        [SerializeField, Range(0, 1)]
        [FormerlySerializedAs("ForceIndex")]
        private int m_ForceIndex;

        /// <summary>
        /// Area near zero where output is not performed
        /// </summary>
        [SerializeField, Range(0, 1)]
        [FormerlySerializedAs("Margin")]
        private float m_Margin = 0.03f;

        [SerializeField, Unchangeable]
        private int m_ForceLimit;

        /// <summary>
        /// Whether to invert the value of the output
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("InvertForce")]
        private bool m_InvertForce;

        [SerializeField, Unchangeable]
        private short m_Force;

        [SerializeField, Unchangeable]
        private float m_ForceRatio;

        #endregion

        /// <summary>
        /// The device using this joint
        /// </summary>
        public ExosDevice Device { get; private set; }

        private bool HasCommandBoard => (Device != null && Device.CommandBoard != null);

        /// <summary>
        /// Mode of motor control
        /// </summary>
        public CommandBoard.ModeMotorControl ModeMotorControl
        {
            get { return (CommandBoard.ModeMotorControl)Device.CommandBoard.MotorControlMode[m_ForceIndex]; }

            set
            {
                if (!HasCommandBoard)
                {
                    Debug.LogWarningFormat("{0} : Device or CommandBoard is null", m_Name);
                    return;
                }

                Device.CommandBoard.MotorControlMode[m_ForceIndex] = (byte)value;

                if (Device.IsConnected) { Device.CommandPort.SendCommandAsync(Command.Short(Device.CommandBoard, CommandBoard.Map.MotorControlModeAll)); }
            }
        }

        /// <summary>
        /// Return the current angle as a ratio
        /// </summary>
        public float AngleRatio
        {
            get
            {
                if (!HasCommandBoard) { return m_AngleRatio; }

                m_Angle = Device.CommandBoard.AnalogReadMapValue[m_AngleIndex];

                if (m_InvertAngle)
                {
                    m_AngleRatio = exiii.Tools.map(m_Angle, m_AngleLimit.x, m_AngleLimit.y, 1.0f, 0.0f);
                }
                else
                {
                    m_AngleRatio = exiii.Tools.map(m_Angle, m_AngleLimit.x, m_AngleLimit.y, 0.0f, 1.0f);
                }

                return m_AngleRatio;
            }
        }

        /// <summary>
        /// Specify the strength of the output as a ratio
        /// </summary>
        public float ForceRatio
        {
            get
            {
                return m_ForceRatio;
            }

            set
            {
                m_ForceRatio = value;

                if (Mathf.Abs(m_ForceRatio) < m_Margin) { m_ForceRatio = 0; }

                m_Force = (short)Mathf.RoundToInt(m_ForceLimit * m_ForceRatio);
                m_Force = (short)Mathf.Clamp(m_Force, -m_ForceLimit, m_ForceLimit);

                if (m_InvertForce)
                {
                    m_ForceRatio *= -1;
                    m_Force *= -1;
                }

                if (HasCommandBoard)
                {
                    Device.CommandBoard.GoalTorque[m_ForceIndex] = m_Force;
                }
            }
        }

        /// <summary>
        /// Initialize joint
        /// </summary>
        /// <param name="device">Using device</param>
        /// <param name="mode">Mode of motor control</param>
        public void Initialize(ExosDevice device, CommandBoard.ModeMotorControl mode)
        {
            Device = device;

            ForceRatio = 0;

            if (HasCommandBoard)
            {
                m_AngleLimit.x = Device.CommandBoard.AnalogMapMin[m_AngleIndex];
                m_AngleLimit.y = Device.CommandBoard.AnalogMapMax[m_AngleIndex];

                m_ForceLimit = Device.CommandBoard.TorqueLimit[m_ForceIndex];


                ModeMotorControl = mode;
            }
        }

        /// <summary>
        /// Confirm whether the value is kept properly
        /// </summary>
        public void OnValidate()
        {            
            m_Name = m_AxisType.EnumToString();
        }

        /// <summary>
        /// Reset angle ratio of this joint
        /// </summary>
        public void ResetRatio()
        {
            // set default ratio.
            m_AngleRatio = m_AngleRatioDefault;
        }
    }
}