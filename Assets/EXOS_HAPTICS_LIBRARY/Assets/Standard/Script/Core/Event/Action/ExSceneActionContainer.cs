using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public class ExSceneActionContainer : ExActionContainer<ESceneActions>
    {
        [SerializeField]
        private List<ExSceneAction> m_Actions = new List<ExSceneAction>();

        protected override IEnumerable<ExActionBase<ESceneActions>> Actions => m_Actions;
    }
}