using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    /// Script for position reference object in prefabuilder
    /// </summary>
    [SelectionBase]
    public class ExosOrigin : ClassifierBase, IExosOrigin
    {
        [SerializeField]
        private EPrefabType m_PrefabType;

        /// <summary>
        /// Type of prefab
        /// </summary>
        public EPrefabType PrefabType
        {
            get { return m_PrefabType; }
        }

        /// <summary>
        /// Tag used by EXOS SDK
        /// </summary>
        public override string ExTag
        {
            get { return exiii.Unity.ExTag.MakeTag(m_PrefabType); }
        }
    }
}