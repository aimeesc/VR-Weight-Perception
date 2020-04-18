using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public class EHLUsableScript : InteractableNode, IUsableScript
    {
        #region Inspector

        [SerializeField]
        [FormerlySerializedAs("m_CanNotGrabbed")]
        private bool m_CanUseNotGrabbed = false;

        [SerializeField]
        private UnityEvent m_OnStart = new UnityEvent();

        [SerializeField]
        private UnityEvent m_OnUpdate = new UnityEvent();

        [SerializeField]
        private UnityEvent m_OnFixedUpdate = new UnityEvent();

        [SerializeField]
        private UnityEvent m_OnEnd = new UnityEvent();

        #endregion Inspector

        public void OnStart(IUseManipulation manipulation)
        {
            if (InteractableRoot.ManipulationState.IsManipulated(EManipulationType.Grab) || m_CanUseNotGrabbed)
            {
                m_OnStart.Invoke();
            }
        }

        public void OnUpdate(IUseManipulation manipulation)
        {
            if (InteractableRoot.ManipulationState.IsManipulated(EManipulationType.Grab) || m_CanUseNotGrabbed)
            {
                m_OnUpdate.Invoke();
            }
        }

        public void OnFixedUpdate(IUseManipulation manipulation)
        {
            if (InteractableRoot.ManipulationState.IsManipulated(EManipulationType.Grab) || m_CanUseNotGrabbed)
            {
                m_OnFixedUpdate.Invoke();
            }
        }

        public void OnEnd(IUseManipulation manipulation)
        {
            if (InteractableRoot.ManipulationState.IsManipulated(EManipulationType.Grab) || m_CanUseNotGrabbed)
            {
                m_OnEnd.Invoke();
            }
        }
    }
}