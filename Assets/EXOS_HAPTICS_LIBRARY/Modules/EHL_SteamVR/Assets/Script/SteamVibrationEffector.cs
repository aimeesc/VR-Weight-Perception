using exiii.Unity.EXOS;
using exiii.Unity.Rx;
using System;
using UnityEngine;
using Valve.VR;

#pragma warning disable 414

namespace exiii.Unity.SteamVR
{
    public class SteamVibrationEffector : InteractorNode, IVibrationEffector
    {
        #region Inspector

        [Header(nameof(SteamVibrationEffector))]
        [SerializeField]
        private SteamVR_Input_Sources m_Source;

        #endregion Inspector

        private SteamVR_Action_Vibration m_Vibration = SteamVR_Input.GetVibrationAction("Haptic");

        protected override void Start()
        {
            if (m_Vibration == null)
            {
                Debug.Log("SteamVR_Action_Vibration is not found");
                enabled = false;
            }

            IObservable<IVibrationState> observable;
            if (InteractorRoot != null && InteractorRoot.TryGetStateObservable(out observable))
            {
                observable.Subscribe(OnVibrate);
            }
        }

        #region IVibrationEffector

        private bool m_Vibrating = false;

        public void OnVibrate(IVibrationState state)
        {
            if (!state.HasVibration || m_Vibrating) { return; }

            var vivration = state.VibrationParameter;

            m_Vibration.Execute(0.0f, vivration.Duration, vivration.Frequency, vivration.Amplitude, m_Source);

            m_Vibrating = true;
                
            Observable.Timer(TimeSpan.FromSeconds(vivration.Duration)).First().Subscribe(_ => m_Vibrating = false);
        }

        #endregion
    }
}

