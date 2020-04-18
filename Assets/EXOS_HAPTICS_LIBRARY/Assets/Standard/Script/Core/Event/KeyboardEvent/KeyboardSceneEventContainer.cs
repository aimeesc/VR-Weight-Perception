using exiii.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace exiii.Unity
{
    public class KeyboardSceneEventContainer : KeyboardEventContainer<ESceneActions>
    {
        #region Inspector

        [Header(nameof(KeyboardSceneEventContainer))]
        [SerializeField]
        private bool AutoSetDescription = true;

        [SerializeField]
        private List<KeyboardSceneEvent> m_KeyboardEvents;

        #endregion Inspector

        protected override void Start()
        {
            base.Start();

            if (AutoSetDescription && m_KeyboardEvents != null)
            {
                m_KeyboardEvents.Foreach(x => SetDescription(x));
            }
        }

        public override void SetTarget()
        {
            var SceneActions = FindObjectsOfType<ExSceneActionContainer>()
                .SelectMany(container => container)
                .ToArray();

            SetDictionaly(SceneActions);
        }

        private void SetDescription(KeyboardSceneEvent @event)
        {
            var index = (int)@event.Action;

            if (index >= SceneManager.sceneCountInBuildSettings) { return; }

            @event.Description = SceneUtility.GetScenePathByBuildIndex(index).Split('/').LastOrDefault();

            /*
            var scene = SceneManager. GetSceneByBuildIndex(index);

            if (scene.IsValid())
            {
                @event.Description = scene.name;
            }
            else
            {
                @event.Description = "";
            }
            */
        }

        #region KeyboardEventContainer

        protected override IEnumerable<KeyboardEventBase<ESceneActions>> Events => m_KeyboardEvents;

        #endregion KeyboardEventContainer
    }
}