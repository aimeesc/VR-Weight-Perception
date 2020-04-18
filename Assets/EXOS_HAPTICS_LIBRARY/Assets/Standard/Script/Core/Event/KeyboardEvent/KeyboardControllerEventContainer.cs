using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public class KeyboardControllerEventContainer : KeyboardEventContainer<EControllerActions>
    {
        #region Inspector

        [Header(nameof(KeyboardControllerEventContainer))]
        [SerializeField]
        private List<KeyboardControllerEvent> m_KeyboardEvents;

        #endregion Inspector

        public override void SetTarget()
        {
            var ControllerActions = FindObjectsOfType<ExControllerActionContainer>()
                .SelectMany(container => container)
                .ToArray();

            SetDictionaly(ControllerActions);
        }

        #region KeyboardEventContainer

        protected override IEnumerable<KeyboardEventBase<EControllerActions>> Events => m_KeyboardEvents;

        #endregion KeyboardEventContainer
    }
}