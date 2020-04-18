using exiii.Unity.Rx;
using System;
using UnityEngine;

namespace exiii.Unity
{
    public class GrabManipulator : EventManipulator<IGrabManipulation, GrabManipulation>, IGrabManipulator
    {
        #region Inspector

        [Header(nameof(GrabManipulator))]
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

            if (m_EventGenerator == null) { return; }

            m_EventGenerator
                .GrabEvent
                .OnStart()
                .ObserveOnMainThread()
                .Subscribe(_ => OnEventManipulationStart())
                .AddTo(this);

            m_EventGenerator
                .GrabEvent
                .OnEnd()
                .ObserveOnMainThread()
                .Subscribe(_ => OnEventManipulationEnd())
                .AddTo(this);
        }

        #region Manipulator

        public override EManipulationType ManipulationType => EManipulationType.Grab;

        protected override GrabManipulation GenerateManipulation(IManipulable<IGrabManipulation> manipulable)
        {
            return new GrabManipulation(InteractorRoot, this, manipulable);
        }

        #endregion Manipulator
    }
}