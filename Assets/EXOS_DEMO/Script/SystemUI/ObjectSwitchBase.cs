using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.UI
{
    public abstract class ObjectSwitchBase : MonoBehaviour
    {
        [SerializeField]
        protected GameObject m_Target;

        public void SwitchEnable()
        {
            m_Target.SetActive(!m_Target.activeSelf);
        }
    }
}