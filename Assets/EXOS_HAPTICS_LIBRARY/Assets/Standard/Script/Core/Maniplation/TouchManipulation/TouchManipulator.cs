using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public sealed class TouchManipulator : ImmidiateManipulator<ITouchManipulation, TouchManipulation>, ITouchManipulator
    {
        #region Inspector

        [Header(nameof(TouchManipulator))]
        [SerializeField, UnchangeableInPlaying]
        [FormerlySerializedAs("samplePoints")]
        private PenetratorContainerBase m_PenetratorContainer;

        [SerializeField]
        [FormerlySerializedAs("m_TouchForceProperty")]
        private TouchForceParameter m_TouchForceParameter;

        public TouchForceParameter TouchForceParameter => m_TouchForceParameter;

        #endregion Inspector

        private IPenetratorContainer m_CurrentPenetratorContainer;

        public IPenetratorContainer PenetratorContainer => m_PenetratorContainer;

        private IPenetratorContainer m_DefaultPenetratorContainer;

        public bool PenetratorIsDefault => m_DefaultPenetratorContainer == m_CurrentPenetratorContainer;

        public override void StartInjection(IRootScript root)
        {
            base.StartInjection(root);

            if (m_PenetratorContainer == null)
            {
                m_PenetratorContainer = RootGameObject.GetComponent<PenetratorContainerBase>();
            }

            m_CurrentPenetratorContainer = m_PenetratorContainer;

            m_DefaultPenetratorContainer = m_PenetratorContainer;
        }

        public void ChangePenetrator(IPenetratorContainer container)
        {
            m_CurrentPenetratorContainer = container;
        }

        public void RestorePenetrator()
        {
            m_CurrentPenetratorContainer = m_DefaultPenetratorContainer;
        }

        #region Manipulator

        public override EManipulationType ManipulationType => EManipulationType.Touch;

        protected override TouchManipulation GenerateManipulation(IManipulable<ITouchManipulation> manipulable)
        {
            return new TouchManipulation(InteractorRoot, this, manipulable);
        }

        #endregion Manipulator
    }
}