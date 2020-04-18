using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public class ExCameraActionContainer : ExActionContainer<ECameraActions>
    {
        [SerializeField]
        private List<ExCameraAction> m_Actions = new List<ExCameraAction>();

        protected override IEnumerable<ExActionBase<ECameraActions>> Actions => m_Actions;
    }
}