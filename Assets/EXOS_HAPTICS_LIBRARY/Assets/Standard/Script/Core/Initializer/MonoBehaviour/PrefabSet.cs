using System;
using UnityEngine;

namespace exiii.Unity
{
    [Serializable]
    public class PrefabSet : IExTag
    {
        #region Inspector

#pragma warning disable 414

        [Header(nameof(PrefabSet))]
        [SerializeField, Unchangeable]
        private string m_Name;

#pragma warning restore 414

        [SerializeField]
        private ExTagContainer m_ExTag;

        [SerializeField, PrefabField]
        private GameObject[] m_Prefabs;

        public GameObject[] Prefabs { get { return m_Prefabs; } }

        #endregion Inspector

        public string ExTag
        {
            get { return m_ExTag.ExTag; }
        }

        public void OnValidate()
        {
            if (m_ExTag != null)
            {
                m_Name = m_ExTag.ExTag;
            }
        }
    }
}