using exiii.Extensions;
using UnityEngine;

namespace exiii.Unity
{
    public abstract class ExTagContainer : ScriptableObject, IExTag
    {
        public abstract string ExTag { get; }
    }

    public abstract class ExTagContainer<TEnum> : ExTagContainer where TEnum : struct
    {
        #region Inspector

        [Header(nameof(ExTagContainer))]
        [SerializeField, Unchangeable]
        private string m_ExTagString;

        [SerializeField]
        private TEnum m_ExTag;

        #endregion Inspector

        public override string ExTag
        {
            get { return m_ExTagString; }
        }

        private void UpdateExTagString()
        {
            m_ExTagString = exiii.Unity.ExTag.MakeTag(m_ExTag);
        }

        private void OnEnable()
        {
            UpdateExTagString();
        }

        private void OnValidate()
        {
            UpdateExTagString();
        }
    }
}