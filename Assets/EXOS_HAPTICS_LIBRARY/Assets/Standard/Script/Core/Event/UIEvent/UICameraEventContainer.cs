using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public class UICameraEventContainer : UIEventContainer<ECameraActions>
    {
        #region Inspector

        [Header(nameof(UICameraEventContainer))]
        [SerializeField]
        private List<UICameraEvent> m_UIEvents;

        #endregion Inspector

        public override void SetTarget()
        {
            var ControllerActions = FindObjectsOfType<ExCameraActionContainer>()
                .SelectMany(container => container)
                .ToArray();

            SetDictionaly(ControllerActions);
        }

        #region UIEventContainer

        protected override IEnumerable<UIEventBase<ECameraActions>> Events => m_UIEvents;

        #endregion UIEventContainer
    }
}