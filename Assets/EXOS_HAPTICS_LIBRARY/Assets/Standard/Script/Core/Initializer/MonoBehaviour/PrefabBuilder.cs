using exiii.Extensions;
using exiii.Unity.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    /// <summary>
    /// Generate the specified Prefab
    /// </summary>
    public class PrefabBuilder : MonoBehaviour
    {
        #region Inspector

        [SerializeField, Unchangeable]
        [FormerlySerializedAs("UniqueID")]
        private string m_UniqueID;

        [SerializeField, PrefabField]
        [FormerlySerializedAs("Objs")]
        private GameObject[] m_Prefabs;

        [SerializeField]
        private PrefabSet[] m_PrefabSets;

        [SerializeField]
        private ExTagContainer[] m_Tags;

        [SerializeField]
        [FormerlySerializedAs("LayerRecursive")]
        private bool m_LayerRecursive = true;

        #endregion Inspector

        public bool BuildFinished { get; private set; } = false;

        private void OnValidate()
        {
            if (m_UniqueID == null)
            {
                Guid guid = Guid.NewGuid();
                m_UniqueID = guid.ToString();
            }

            m_PrefabSets.CheckNull().Foreach(x => x.OnValidate());
        }

        private void Start()
        {
            Build();
        }

        public void Build()
        {
            Build(null);
        }

        /// <summary>
        /// Generate the specified Prefab
        /// </summary>
        public void Build(IEnumerable<IExTag> tags = null)
        {
            if (BuildFinished) { return; }

            var infinitiLoop = gameObject
                .Ancestors()
                .Select(obj => obj.GetComponent<PrefabBuilder>())
                .CheckNull()
                .Where(obj => obj.m_UniqueID == m_UniqueID)
                .Any();

            if (infinitiLoop)
            {
                Debug.LogError("[EXOS_SDK] PrefabBuilder : 親に同一のオブジェクトが含まれています", gameObject);
                return;
            }

            IEnumerable<GameObject> prefabs = m_Prefabs;

            if (tags == null)
            {
                tags = m_Tags;
            }
            else
            {
                tags = tags.Concat(m_Tags);
            }

            m_PrefabSets
                .Where(x => x.CheckTag(tags))
                .Foreach(x => prefabs = prefabs.Concat(x.Prefabs));

            var instances = prefabs
                .CheckNull()
                .Select(obj => Instantiate(obj))
                .ToArray();

            // set layer recursive.
            if (m_LayerRecursive)
            {
                instances
                    .Foreach(instance => instance
                        .DescendantsAndSelf()
                        .Foreach(descendant => descendant.layer = gameObject.layer)
                    );
            }

            instances
                .Select(instance => instance.GetComponent<PrefabBuilder>())
                .CheckNull()
                .Foreach(builder => builder.Build(tags));

            BuildFinished = true;
        }

        private GameObject Instantiate(GameObject obj)
        {
            var instance = Instantiate(obj, transform);

            instance.name = obj.name;

            return instance;
        }
    }
}