using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public class EHLGrabbableScript : InteractableNode, IGrabbableScript
    {
        #region Inspector

        [SerializeField]
        private UnityEvent m_OnStart = new UnityEvent();

        [SerializeField]
        private UnityEvent m_OnUpdate = new UnityEvent();

        [SerializeField]
        private UnityEvent m_OnFixedUpdate = new UnityEvent();

        [SerializeField]
        private UnityEvent m_OnEnd = new UnityEvent();

        #endregion Inspector

        public void OnStart(IGrabManipulation manipulation)
        {
            if (InteractableRoot.ManipulationState.IsManipulated(EManipulationType.Grab))
            {
                m_OnStart.Invoke();
            }
        }

        public void OnUpdate(IGrabManipulation manipulation)
        {
            if (InteractableRoot.ManipulationState.IsManipulated(EManipulationType.Grab))
            {
                m_OnUpdate.Invoke();
            }
        }

        public void OnFixedUpdate(IGrabManipulation manipulation)
        {
            if (InteractableRoot.ManipulationState.IsManipulated(EManipulationType.Grab))
            {
                m_OnFixedUpdate.Invoke();
            }
        }

        public void OnEnd(IGrabManipulation manipulation)
        {
            if (InteractableRoot.ManipulationState.IsManipulated(EManipulationType.Grab))
            {
                m_OnEnd.Invoke();
            }
        }
    }
}