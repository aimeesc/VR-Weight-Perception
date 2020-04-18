using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public class KeyboardCameraEventContainer : KeyboardEventContainer<ECameraActions>
    {
        #region Inspector

        [Header(nameof(KeyboardCameraEventContainer))]
        [SerializeField]
        private List<KeyboardCameraEvent> m_KeyboardEvents;

        #endregion Inspector

        public override void SetTarget()
        {
            var ControllerActions = FindObjectsOfType<ExCameraActionContainer>()
                .SelectMany(container => container)
                .ToArray();

            SetDictionaly(ControllerActions);
        }

        #region KeyboardEventContainer

        protected override IEnumerable<KeyboardEventBase<ECameraActions>> Events => m_KeyboardEvents;

        #endregion KeyboardEventContainer
    }
}