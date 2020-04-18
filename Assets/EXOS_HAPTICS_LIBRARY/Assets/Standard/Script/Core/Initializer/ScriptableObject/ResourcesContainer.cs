using exiii.Extensions;

//using System.Reactive.Linq;
using exiii.Unity.Rx;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    [CreateAssetMenu(fileName = "ResourcesContainer", menuName = "EXOS/System/ResourcesContainer")]
    public class ResourcesContainer : ResourcesHolder<ResourcesContainer>
    {
        #region Static

        static ResourcesContainer()
        {
            AssetName = nameof(ResourcesContainer);
            DoNameCheck = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            Instances.Foreach(x => x.Initialize());
        }

        #endregion Static

        #region Inspector

        [SerializeField]
        public List<ExScriptableObject> InitializeOnLoadObjects;

        #endregion Inspector

        private void Initialize()
        {
            EHLDebug.Log("ResourcesContainer.InitializeOnLoad[Start]", this);

            InitializeOnLoadObjects
                .CheckNull()
                .Foreach(x => x.Initialize());

            Observable.OnceApplicationQuit().Subscribe(_ => Termination());

            EHLDebug.Log("ResourcesContainer.InitializeOnLoad[Finish]", this);
        }

        private void Termination()
        {
            EHLDebug.Log("ResourcesContainer.Termination[Start]", this);

            InitializeOnLoadObjects
                .CheckNull()
                .Reverse()
                .Foreach(x => x.Terminate());

            EHLDebug.Log("ResourcesContainer.Termination[Finish]", this);
        }
    }
}