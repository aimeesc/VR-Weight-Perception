using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public class ExDebugActionContainer : ExActionContainer<EDebugActions>
    {
        [SerializeField]
        private List<ExDebugAction> m_Actions = new List<ExDebugAction>();

        protected override IEnumerable<ExActionBase<EDebugActions>> Actions => m_Actions;
    }
}