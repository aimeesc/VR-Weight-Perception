using exiii.Unity.Device;
using exiii.Unity.Rx;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace exiii.Unity
{
    public class CheckDeviceConnection : MonoBehaviour
    {
        [SerializeField]
        private ExosDevice[] m_Devices;

        [SerializeField]
        private UnityEvent m_AfterConnected = new UnityEvent();

        void Start()
        {
            foreach (var device in m_Devices)
            {
                if (device.IsConnected)
                {
                    m_AfterConnected.Invoke();
                }
                else
                {
                    Observable
                        .Interval(TimeSpan.FromSeconds(1.0))
                        .Where(_ => device.IsConnected)
                        .First()
                        .Subscribe(_ => m_AfterConnected.Invoke())
                        .AddTo(this);
                }
            }
        }
    }
}
