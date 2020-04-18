using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public class ExControllerActionContainer : ExActionContainer<EControllerActions>
    {
        [SerializeField]
        private List<ExControllerAction> m_Actions = new List<ExControllerAction>();

        protected override IEnumerable<ExActionBase<EControllerActions>> Actions => m_Actions;
    }
}