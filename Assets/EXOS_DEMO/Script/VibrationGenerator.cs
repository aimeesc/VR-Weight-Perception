using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class VibrationGenerator : MonoBehaviour, ITouchableScript, ITouchVibrationGenerator
    {
        #region Inspector

        [SerializeField]
        private VibrationParameter m_VibrationParameter;       

        #endregion

        private bool m_Active = false;

        public void OnStart(ITouchManipulation manipulation)
        {
            m_Active = true;
        }

        public void OnUpdate(ITouchManipulation manipulation)
        {
            
        }

        public void OnFixedUpdate(ITouchManipulation manipulation)
        {
            
        }

        public void OnEnd(ITouchManipulation manipulation)
        {
            
        }

        public void OnGenerate(IVibrationReceiver receiver, ITouchState state)
        {
            if (m_Active)
            {
                receiver.AddVibration(m_VibrationParameter);
                m_Active = false;
            }
        }
    }
}

