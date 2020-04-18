using exiii.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace exiii.Unity
{
    public class UISceneEventContainer : UIEventContainer<ESceneActions>
    {
        #region Inspector

        [Header(nameof(UISceneEventContainer))]
        [SerializeField]
        private bool AutoSetDescription = true;

        [SerializeField]
        private List<UISceneEvent> m_UIEvents;

        #endregion Inspector

        protected override void Start()
        {
            base.Start();

            if (AutoSetDescription && m_UIEvents != null)
            {
                m_UIEvents.Foreach(x => SetDescription(x));
            }
        }

        public override void SetTarget()
        {
            var SceneActions = FindObjectsOfType<ExSceneActionContainer>()
                .SelectMany(container => container)
                .ToArray();

            SetDictionaly(SceneActions);
        }

        private void SetDescription(UISceneEvent @event)
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

        public void ChangeToScene(int index)
        {
            switch(index)
            {
                case 0:
                    Emit(ESceneActions.Scene0);
                    break;

                case 1:
                    Emit(ESceneActions.Scene1);
                    break;

                case 2:
                    Emit(ESceneActions.Scene2);
                    break;

                case 3:
                    Emit(ESceneActions.Scene3);
                    break;

                case 4:
                    Emit(ESceneActions.Scene4);
                    break;

                case 5:
                    Emit(ESceneActions.Scene5);
                    break;

                case 6:
                    Emit(ESceneActions.Scene6);
                    break;

                case 7:
                    Emit(ESceneActions.Scene7);
                    break;

                case 8:
                    Emit(ESceneActions.Scene8);
                    break;

                case 9:
                    Emit(ESceneActions.Scene9);
                    break;
            }
        }

        #region UIEventContainer

        protected override IEnumerable<UIEventBase<ESceneActions>> Events => m_UIEvents;

        #endregion UIEventContainer
    }
}