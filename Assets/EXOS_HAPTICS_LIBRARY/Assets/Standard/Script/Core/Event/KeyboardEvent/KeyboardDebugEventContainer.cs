using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public class KeyboardDebugEventContainer : KeyboardEventContainer<EDebugActions>
    {
        #region Inspector

        [Header(nameof(KeyboardDebugEventContainer))]
        [SerializeField]
        private List<KeyboardDebugEvent> m_KeyboardEvents;

        #endregion Inspector

        public override void SetTarget()
        {
            var SceneActions = FindObjectsOfType<ExDebugActionContainer>()
                .SelectMany(container => container)
                .ToArray();

            SetDictionaly(SceneActions);
        }

        #region KeyboardEventContainer

        protected override IEnumerable<KeyboardEventBase<EDebugActions>> Events => m_KeyboardEvents;

        #endregion KeyboardEventContainer
    }
}