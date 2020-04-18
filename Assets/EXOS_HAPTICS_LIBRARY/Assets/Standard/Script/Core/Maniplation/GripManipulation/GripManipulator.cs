using exiii.Unity.Rx;
using System;
using UnityEngine;

namespace exiii.Unity
{
    public class GripManipulator : CycleManipulator<IGripManipulation, GripManipulation>, IGripManipulator
    {
        #region Inspector

        [Header(nameof(GripManipulator))]
        [SerializeField]
        private GripBehaviourBase m_GripController;

        public IGripBehaviour GripController
        {
            get { return m_GripController; }
        }

        #endregion Inspector

        protected override void Start()
        {
            base.Start();

            if (m_GripController == null) { return; }

            m_GripController
                .GripEvent
                .OnStart()
                .ObserveOnMainThread()
                .Subscribe(OnManipulationStart)
                .AddTo(this);

            m_GripController
                .GripEvent
                .OnEnd()
                .ObserveOnMainThread()
                .Subscribe(OnManipulationEnd)
                .AddTo(this);
        }

        #region Manipulator

        public override EManipulationType ManipulationType => EManipulationType.Grip;

        protected override GripManipulation GenerateManipulation(IManipulable<IGripManipulation> manipulable)
        {
            return new GripManipulation(InteractorRoot, this, manipulable);
        }

        #endregion Manipulator
    }
}