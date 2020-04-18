using exiii.Extensions;
using exiii.Unity.Rx.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    /// <summary>
    /// Generate the specified GameObject at application startup
    /// </summary>
    [CreateAssetMenu(fileName = "ObjectGenerator", menuName = "EXOS/System/ObjectGenerator")]
    public class ObjectGenerator : ExScriptableObject
    {
        #region Inspector

        [Header(nameof(ObjectGenerator))]
        [SerializeField]
        [FormerlySerializedAs("Enable")]
        private bool m_Enable = true;

        [SerializeField]
        [FormerlySerializedAs("DebugBuildOnly")]
        private bool m_DebugBuildOnly = false;

        [SerializeField]
        [FormerlySerializedAs("DontDestroy")]
        private bool m_DontDestroy;

        [SerializeField, PrefabField]
        [FormerlySerializedAs("Prefabs")]
        private ExMonoBehaviour[] m_Prefabs;

        #endregion Inspector

        private List<ExMonoBehaviour> m_InstanceList = new List<ExMonoBehaviour>();

        public override void Initialize()
        {
            base.Initialize();

            if (!m_Enable) { return; }

            if (m_DebugBuildOnly && !Debug.isDebugBuild) { return; }

            Debug.Log($"[EXOS_SDK] ObjectGenerator is running : {ExName}", this);

            m_InstanceList.Clear();

            foreach (var prefab in m_Prefabs)
            {
                if (prefab == null) { continue; }

                var exObj = Instantiate(prefab);
                exObj.gameObject.name = prefab.gameObject.name;

                m_InstanceList.Add(exObj);

                exObj.Initialize();

                if (m_DontDestroy) { DontDestroyOnLoad(exObj); }

                exObj.OnDestroyAsObservable()
                    .Subscribe(x => { exObj.Terminate(); });
            }
        }

        public override void Terminate()
        {
            base.Terminate();

            IEnumerable<ExMonoBehaviour> instances = m_InstanceList as IEnumerable<ExMonoBehaviour>;
            if (instances != null)
            {
                instances
                    .Reverse()
                    .CheckNull()
                    .Foreach(x => x.Terminate());

                m_InstanceList.Clear();
            }
        }
    }
}