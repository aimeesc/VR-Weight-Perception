using exiii.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace exiii.Unity
{
    /// <summary>
    /// Manager class that controls execution of Prefab Builder
    /// </summary>
    [CreateAssetMenu(fileName = "SceneInitializer", menuName = "EXOS/System/SceneInitializer")]
    public class SceneInitializer : ExScriptableObject
    {
        /// <summary>
        /// Called with Initialize
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Called with Termination
        /// </summary>
        public override void Terminate()
        {
            base.Terminate();

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
        {
            TargetSceneBuildStart(loadedScene);
        }

        private void TargetSceneBuildStart(Scene targetScene)
        {
            EHLDebug.Log($"{nameof(SceneInitializer)}.BuildStart", this);

            var prefabBuilders = ExtensionUnity.FindObjectsOfInterface<PrefabBuilder>()
                .Where(obj => obj.gameObject.scene == targetScene);

            if (EHLHand.IsExist)
            {
                List<IExTag> list = new List<IExTag>();
                list.Add(new ExTag<EHandMode>(EHLHand.Instance.HandMode));
                list.Add(new ExTag<EDeviceEquipType>(EHLHand.Instance.DeviceEquipType));

                prefabBuilders
                    .Where(x => !x.BuildFinished)
                    .Foreach(builder => { builder.Build(list); });
            }
            else
            {
                prefabBuilders
                    .Where(x => !x.BuildFinished)
                    .Foreach(builder => { builder.Build(); });
            }

            var roots = ExtensionUnity.FindObjectsOfInterface<RootScript>()
                .Where(obj => obj.gameObject.scene == targetScene);

            roots.Foreach(root => { root.BuildHierarchy(); });
            roots.Foreach(root => { root.RootInjection(); });

            EHLDebug.Log($"{nameof(SceneInitializer)}.BuildFinish", this);
        }
    }
}