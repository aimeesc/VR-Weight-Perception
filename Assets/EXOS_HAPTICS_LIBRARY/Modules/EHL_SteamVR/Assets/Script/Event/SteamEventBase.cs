using System;
using UnityEngine;

namespace exiii.Unity.SteamVR
{
    [Serializable]
    public abstract class SteamEventBase<TAction>
    {
        #region Inspector

        [SerializeField, HideInInspector]
        private string m_Name;

        public string Name => m_Name;

        [SerializeField, UnchangeableInPlaying]
        private TAction m_Action;

        public TAction Action => m_Action;

        [SerializeField, UnchangeableInPlaying]
        private EManipulationType m_ManipulationType;

        public EManipulationType ManipulationType => m_ManipulationType;

        [SerializeField, UnchangeableInPlaying]
        private EKeyTiming m_KeyTiming;

        public EKeyTiming KeyTiming => m_KeyTiming;

        [SerializeField]
        private String m_Description;

        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        #endregion Inspector

        public void UpdateName()
        {
            m_Name = m_Action.ToString();
        }
    }
}

