using UnityEngine;

namespace exiii.Unity.Sample
{
    public class ChangePenetrator : InteractableNode, IGrabbableScript
    {
        [SerializeField]
        private PenetratorContainerBase m_PenetratorContainer;

        public void OnStart(IGrabManipulation manipulation)
        {
            ITouchManipulator Toucher;

            if (manipulation.InteractorRoot.TryGetManipulator(out Toucher))
            {
                Toucher.ChangePenetrator(m_PenetratorContainer);
            }
        }

        public void OnUpdate(IGrabManipulation manipulation)
        {

        }

        public void OnFixedUpdate(IGrabManipulation manipulation)
        {
            
        }

        public void OnEnd(IGrabManipulation manipulation)
        {
            ITouchManipulator Toucher;

            if (manipulation.InteractorRoot.TryGetManipulator(out Toucher))
            {
                Toucher.RestorePenetrator();
            }
        }
    }
}