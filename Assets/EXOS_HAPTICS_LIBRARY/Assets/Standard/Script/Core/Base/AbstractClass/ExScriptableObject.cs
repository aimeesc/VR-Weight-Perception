using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public abstract class ExScriptableObject : ScriptableObject, IExObject
    {
        #region Inspector

        [Header(nameof(ExScriptableObject))]
        [SerializeField, Unchangeable]
        private string m_ExName;

        public string ExName => m_ExName;

        [SerializeField]
        private ExTagContainer[] m_ExTags;

        public IReadOnlyCollection<IExTag> ExTags { get { return m_ExTags; } }

        [SerializeField, Unchangeable]
        private bool m_IsActive;

        public bool IsActive => m_IsActive;

        #endregion Inspector

        protected virtual void OnValidate()
        {
            UpdateName();
        }

        protected virtual void Awake()
        {
            UpdateName();
        }

        public virtual void Initialize()
        {
            m_IsActive = true;
        }

        public virtual void Terminate()
        {
            m_IsActive = false;
        }

        [ContextMenu("UpdateName")]
        public void UpdateName()
        {
            m_ExName = name;
        }
    }
}