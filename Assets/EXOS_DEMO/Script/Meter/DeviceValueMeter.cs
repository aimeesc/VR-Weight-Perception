using exiii.Unity.Device;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Sample
{
    enum MeterType
    {
        Force,
        Angle,
    }

	public class DeviceValueMeter : MonoBehaviour
	{
        [SerializeField]
        private ExosDevice m_Device;

        [SerializeField]
        private EAxisType m_AxisType;

        private ExosJoint m_Joint;

        [SerializeField]
        private MeterType MeterType;

        [SerializeField]
        private LineRenderer m_Line;

        private float MeterLength = 0.1f;

        private void Start()
        {
            StartAsync();
        }

        private async void StartAsync()
        {
            if (m_Device == null || m_Line == null)
            {
                enabled = false;
                return;
            }

            if (!m_Device.Enabled)
            {
                await m_Device.InitializeAsync();
            }

            if (!m_Device.IsConnected)
            {
                enabled = false;
                return;
            }

            if (!m_Device.TryGetJoint(m_AxisType, out m_Joint))
            {
                enabled = false;
                return;
            }

            m_Line.useWorldSpace = false;
            m_Line.SetPosition(0, Vector3.zero);
        }

        private void Update()
        {
            if (m_Joint == null || m_Line == null) { return; }

            switch(MeterType)
            {
                case MeterType.Force:
                    m_Line.SetPosition(0, transform.up * MeterLength * m_Joint.ForceRatio);
                    break;

                case MeterType.Angle:
                    m_Line.SetPosition(0, transform.up * MeterLength * m_Joint.AngleRatio);
                    break;
            }            
        }

        /*
        private void OnDestroy()
        {
            if (m_Device == null) { return; }

            if (m_Device.Enabled)
            {
                m_Device.Termination();
            }
        }
        */
    }
}
