using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public class EHLTouchableScript : InteractableNode, ITouchableScript
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

        public void OnStart(ITouchManipulation manipulation)
        {
            m_OnStart.Invoke();
        }

        public void OnUpdate(ITouchManipulation manipulation)
        {
            m_OnUpdate.Invoke();
        }

        public void OnFixedUpdate(ITouchManipulation manipulation)
        {
            m_OnFixedUpdate.Invoke();
        }

        public void OnEnd(ITouchManipulation manipulation)
        {
            m_OnEnd.Invoke();
        }
    }
}