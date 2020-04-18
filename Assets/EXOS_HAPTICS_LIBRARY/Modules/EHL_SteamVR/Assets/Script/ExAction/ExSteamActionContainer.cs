using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.SteamVR
{
    public class ExSteamActionContainer : ExActionContainer<EVRControllerActions>
    {
        [SerializeField]
        private List<ExSteamAction> m_Actions = new List<ExSteamAction>();

        protected override IEnumerable<ExActionBase<EVRControllerActions>> Actions => m_Actions;
    }
}

