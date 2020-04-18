using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public class UIDebugEventContainer : UIEventContainer<EDebugActions>
    {
        #region Inspector

        [Header(nameof(UIDebugEventContainer))]
        [SerializeField]
        private List<UIDebugEvent> m_UIEvents;

        #endregion Inspector

        public override void SetTarget()
        {
            var SceneActions = FindObjectsOfType<ExDebugActionContainer>()
                .SelectMany(container => container)
                .ToArray();

            SetDictionaly(SceneActions);
        }

        #region KeyboardEventContainer

        protected override IEnumerable<UIEventBase<EDebugActions>> Events => m_UIEvents;

        #endregion KeyboardEventContainer
    }
}