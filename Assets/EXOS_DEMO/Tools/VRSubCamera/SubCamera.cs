using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace exiii.Unity.XR
{
    public class SubCamera : MonoBehaviour
    {
        void OnEnable()
        {
            XRSettings.showDeviceView = false;
        }

        void OnDisable()
        {
            XRSettings.showDeviceView = true;
        }
    }
}