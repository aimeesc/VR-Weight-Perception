using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity.UI
{
    public class ObjectSwitchGrip : ObjectSwitchBase, ISizeEffector
    {
        [SerializeField]
        float m_OnThreashold = 0.7f;

        [SerializeField]
        float m_ReleaseThreashold = 0.9f;

        [SerializeField, Unchangeable]
        float m_SizeRatio;

        bool m_flug = false;

        public void OnChangeSizeRatio(ISizeState state)
        {
            m_SizeRatio = state.SizeRatio;

            if (!m_flug && state.SizeRatio < m_OnThreashold)
            {
                m_flug = true;
                SwitchEnable();
            }

            if (m_flug && state.SizeRatio > m_ReleaseThreashold)
            {
                m_flug = false;
            }
        }

        public void OnResetSizeRatio()
        {
            m_SizeRatio = 1;
        }
    }
}