using exiii.Unity.Device;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Sample
{
	public class DeviceValueChanger : MonoBehaviour
	{
        [SerializeField]
        private ExosDevice m_Device;

        [SerializeField]
        private EAxisType m_AxisType;

        private ExosJoint m_Joint;

        [SerializeField, Range(-1,1)]
        private float m_ForceRatio = 0;

        private async void Start()
        {
            if (m_Device == null)
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
        }

        private void Update()
        {
            if (m_Joint == null) { return; }

            m_Joint.ForceRatio = m_ForceRatio;
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
