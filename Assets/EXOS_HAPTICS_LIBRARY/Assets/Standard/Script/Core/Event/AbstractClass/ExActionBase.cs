using System;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 414

namespace exiii.Unity
{
    [Serializable]
    public abstract class ExActionBase<TAction>
    {
        [SerializeField, HideInInspector]
        private string m_Name;

        [SerializeField]
        private TAction m_Action;

        public TAction Action => m_Action;

        [SerializeField]
        private UnityEvent m_Events;

        public UnityEvent Events => m_Events;

        public void OnValidate()
        {
            m_Name = m_Action.ToString();
        }
    }
}