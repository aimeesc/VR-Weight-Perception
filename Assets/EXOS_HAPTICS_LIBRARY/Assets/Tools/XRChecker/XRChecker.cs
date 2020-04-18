using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

#pragma warning disable 414

namespace exiii.Unity.XR
{
    public class XRChecker : MonoBehaviour
    {
        [SerializeField]
        private string m_XRModel;

        void OnValidate()
        {
            m_XRModel = XRDevice.model;
        }
    }
}