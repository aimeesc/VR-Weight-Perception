using exiii.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity.SteamVR
{
    public class SteamActionEventContainer : SteamEventContainer<EVRControllerActions>
    {
        #region Inspector

        [Header(nameof(SteamActionEventContainer))]
        [SerializeField]
        private List<SteamActionEvent> m_SteamEvents;

        #endregion Inspector

        public override void UpdateTarget()
        {
            var ControllerActions = FindObjectsOfType<ExSteamActionContainer>()
                .SelectMany(container => container)
                .ToArray();

            SetDictionaly(ControllerActions);
        }

        protected override void UpdateSteamEventAction()
        {
            var gen = GetComponent<SteamEHLEventGenerator>();
            if (gen == null) { return; }
            
            // check and invoke events.
            SteamEvents
                .Where(_ => _.KeyTiming == EKeyTiming.KeyDown && gen.IsStateDown(_.ManipulationType))
                .Where(_ => Dictionaly.ContainsKey(_.Action))
                .Foreach(_ => EventInvoke(Dictionaly[_.Action]));
            SteamEvents
                .Where(_ => _.KeyTiming == EKeyTiming.Key && gen.IsStateStay(_.ManipulationType))
                .Where(_ => Dictionaly.ContainsKey(_.Action))
                .Foreach(_ => EventInvoke(Dictionaly[_.Action]));
            SteamEvents
                .Where(_ => _.KeyTiming == EKeyTiming.KeyUp && gen.IsStateUp(_.ManipulationType))
                .Where(_ => Dictionaly.ContainsKey(_.Action))
                .Foreach(_ => EventInvoke(Dictionaly[_.Action]));
        }

        #region SteamEventContainer

        protected override IEnumerable<SteamEventBase<EVRControllerActions>> SteamEvents => m_SteamEvents;

        #endregion SteamEventContainer
    }
}
