using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public class ColliderPenetratorContainer : InteractorNode, IPenetratorContainer
    {
        #region Inspector

        [SerializeField]
        private Collider[] m_Colliders;

        private List<IPenetrator> m_Penetrators = new List<IPenetrator>();

        #endregion Inspector

        public override void Initialize()
        {
            base.Initialize();

            if (m_Colliders == null || m_Penetrators == null)
            {
                enabled = false;
                return;
            }

            if (m_Colliders.Length == 0)
            {
                m_Colliders = GetComponentsInChildren<Collider>();
            }

            foreach (var collider in m_Colliders)
            {
                m_Penetrators.Add(new ColliderPenetrator(collider));
            }
        }

        #region IPenetratorContainer

        public IReadOnlyCollection<IPenetrator> Penetrators => m_Penetrators;

        #endregion IPenetratorContainer
    }
}