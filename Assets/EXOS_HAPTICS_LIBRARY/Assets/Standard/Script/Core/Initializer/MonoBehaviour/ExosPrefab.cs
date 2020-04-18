using exiii.Extensions;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    /// <summary>
    /// Script for build object in prefabuilder
    /// </summary>
    // [SelectionBase]
    public class ExosPrefab : ClassifierBase, IExosPrefab
    {
        #region Inspector

        [Header(nameof(ExosPrefab))]
        [SerializeField]
        private EPrefabType m_PrefabType;

        /// <summary>
        /// Type of prefab
        /// </summary>
        public EPrefabType PrefabType { get { return m_PrefabType; } }

        [SerializeField]
        private bool m_MoveOnBuild = false;

        [SerializeField]
        private bool m_SaveTransformValuesOnInit = true;

        [SerializeField]
        [FormerlySerializedAs("m_TransformParameter")]
        private TransformValues m_TransformValues = new TransformValues();

#pragma warning disable 0414

        [Header("DebugInspector")]
        [SerializeField]
        private GameObject[] m_PrefabOrigins;

        [SerializeField, Unchangeable]
        private bool m_IsBuilded = false;

#pragma warning restore 0414

        #endregion Inspector

#if UNITY_EDITOR

        private void OnValidate()
        {
            m_PrefabOrigins = GetComponentsInChildren<ExosOrigin>().Select(x => x.gameObject).ToArray();
        }

#endif

        private void Awake()
        {
            if (m_SaveTransformValuesOnInit)
            {
                m_TransformValues.LocalsSaveFrom(transform);
            }
            else
            {
                m_TransformValues.ScalesSaveFrom(transform);
            }
        }

        private bool TrySearchOrigin(RootScript root, out IExosOrigin trans)
        {
            var origins = root
                .GetComponentsInChildren<IExosOrigin>()
                .Where(x => x.PrefabType == m_PrefabType);

            if (origins.Count() == 0)
            {
                Debug.LogWarning($"target ExosOrigin[{m_PrefabType.EnumToString()}] is not found");

                trans = null;
                return false;
            }
            else if (origins.Count() >= 2)
            {
                Debug.LogWarning($"{root} contains 2 more same ExosOrigin[{m_PrefabType.EnumToString()}], use first");
            }

            trans = origins.First();
            return true;
        }

        #region IExosPrefab

        /// <summary>
        /// Tag used by EXOS SDK
        /// </summary>
        public override string ExTag
        {
            get { return exiii.Unity.ExTag.MakeTag(m_PrefabType); }
        }

        /// <summary>
        /// Function called at the timing of generating an object in the prefab builder
        /// </summary>
        public void BuildHierarchy(RootScript root)
        {
            EHLDebug.Log($"ExosPrefab.BuildHierarchy : {name}", this);

            if (m_MoveOnBuild)
            {
                IExosOrigin origin;

                TrySearchOrigin(root, out origin);

                if (origin != null)
                {
                    transform.parent = origin.transform;
                }
                else
                {
                    Debug.LogError("MoveOnBuild target is not found.", this);
                }
            }

            this.UpdateAsObservable().Subscribe(_ => m_TransformValues.LocalsLoadTo(transform));

            m_IsBuilded = true;
        }

        #endregion IExosPrefab
    }
}