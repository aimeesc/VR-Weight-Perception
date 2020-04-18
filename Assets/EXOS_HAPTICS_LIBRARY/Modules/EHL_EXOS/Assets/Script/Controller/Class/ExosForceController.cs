using exiii.Unity.Device;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.EXOS
{
    public class ExosForceController : ForceController, IExosForceReceiver, IExosForceState
    {
        private ExosDevice Device { get; }

        private List<ExosForce> m_DirectForceList = new List<ExosForce>();
        public IReadOnlyCollection<ExosForce> DirectForceList => m_DirectForceList;

        private Dictionary<EAxisType, EForceMode> m_ForceModeDic = new Dictionary<EAxisType, EForceMode>();
        public IReadOnlyDictionary<EAxisType, EForceMode> ForceModeDic => m_ForceModeDic;

        public ExosForceController(TransformState state, ExosDevice device) : base(state)
        {
            Device = device;
        }

        public override void ResetVector()
        {
            base.ResetVector();

            m_DirectForceList.Clear();
            m_ForceModeDic.Clear();
        }

        public void AddDirectForceRatio(EAxisType axis, float forceRatio)
        {
            // Hack: Shuld change method for reverse
            if (Device.ReverseFromDirectInput)
            {
                m_DirectForceList.Add(new ExosForce(axis, -forceRatio));
            }
            else
            {
                m_DirectForceList.Add(new ExosForce(axis, forceRatio));
            }
        }

        public void ChangeForceMode(EAxisType axis, EForceMode mode)
        {
            if (m_ForceModeDic.ContainsKey(axis)) { m_ForceModeDic.Remove(axis); }

            m_ForceModeDic.Add(axis, mode);
        }
    }
}

