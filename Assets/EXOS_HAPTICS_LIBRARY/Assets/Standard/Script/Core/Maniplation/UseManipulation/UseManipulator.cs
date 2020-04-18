using exiii.Unity.Rx;
using System;
using UnityEngine;

namespace exiii.Unity
{
    public class UseManipulator : EventManipulator<IUseManipulation, UseManipulation>, IUseManipulator
    {
        #region Inspector

        [Header(nameof(UseManipulator))]
        [SerializeField]
        private EHLEventGeneratorBase m_EventGenerator;

        public EHLEventGeneratorBase EventGenerator
        {
            get { return m_EventGenerator; }
        }

        #endregion Inspector

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_EventGenerator == null)
            {
                m_EventGenerator = GetComponentInChildren<EHLEventGeneratorBase>();
            }
        }

#endif

        protected override void Start()
        {
            base.Start();

            if (m_EventGenerator == null)
            {
                m_EventGenerator = GetComponentInChildren<EHLEventGeneratorBase>();
            }

            EventGenerator
                .UseEvent
                .OnStart()
                .ObserveOnMainThread()
                .Subscribe(_ => OnEventManipulationStart())
                .AddTo(this);

            EventGenerator
                .UseEvent
                .OnEnd()
                .ObserveOnMainThread()
                .Subscribe(_ => OnEventManipulationEnd())
                .AddTo(this);
        }

        #region Manipulator

        public override EManipulationType ManipulationType => EManipulationType.Use;

        protected override UseManipulation GenerateManipulation(IManipulable<IUseManipulation> manipulable)
        {
            return new UseManipulation(InteractorRoot, this, manipulable);
        }

        #endregion Manipulator
    }
}