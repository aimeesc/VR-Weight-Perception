using System;
using UnityEngine;

namespace exiii.Unity
{
    [CreateAssetMenu(fileName = "VibrationParameter", menuName = "EXOS/Parameter/Vibration")]
    public class VibrationParameter : ParameterAsset<VibrationParameter>, IVibrationParameter
    {
        #region Inspector

        [Header(nameof(VibrationParameter))]
        [SerializeField]
        private float m_Duration = 0.02f;

        public float Duration { get { return m_Duration; } }

        [SerializeField]
        private float m_Frequency = 300f;

        public float Frequency { get { return m_Frequency; } }

        [SerializeField]
        private float m_Amplitude = 0.3f;

        public float Amplitude { get { return m_Amplitude; } }

        #endregion Inspector

        protected override void Reset()
        {
            base.Reset();

            m_Duration = 0.02f;
            m_Frequency = 300f;
            m_Amplitude = 0.3f;
        }

        public override VibrationParameter CreateCopy(UnityEngine.Object owner)
        {
            var instance = Instantiate(this);

            instance.Owner = owner;
            instance.IsOriginal = false;

            instance.m_Duration = this.m_Duration;
            instance.m_Frequency = this.m_Frequency;
            instance.m_Amplitude = this.m_Amplitude;

            return instance;
        }
    }
}