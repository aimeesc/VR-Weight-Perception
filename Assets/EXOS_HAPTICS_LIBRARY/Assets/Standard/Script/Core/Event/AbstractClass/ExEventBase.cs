using UnityEngine;
using System.Collections;
using System;

namespace exiii.Unity
{
    [Serializable]
    public class ExEventBase<TAction>
	{
		#region Inspector

		[Header("ExEventBase")]
        [SerializeField, HideInInspector]
        private string m_Name;

        public string Name => m_Name;

        [SerializeField]
        private string m_Description;

        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        [SerializeField, UnchangeableInPlaying]
        private TAction m_Action;

        public TAction Action => m_Action;

        #endregion Inspector

        public void UpdateName()
        {
            m_Name = m_Action.ToString();
        }
    }
}
