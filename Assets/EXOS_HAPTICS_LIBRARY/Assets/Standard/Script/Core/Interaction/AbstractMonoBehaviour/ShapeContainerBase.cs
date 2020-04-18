using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace exiii.Unity
{
    public abstract class ShapeContainerBase : InteractableNode, IShapeContainer
    {
        public virtual bool HasShapeData { get { return true; } }

        private Collider[] m_Colliders;

        public override void Initialize()
        {
            base.Initialize();

            m_Colliders = RootGameObject.GetComponentsInChildren<Collider>();
        }

        public virtual void CalcShapeStateSet(IEnumerable<IPenetrator> penetrators, ShapeStateSet stateSet)
        {
            if (penetrators == null || penetrators.Count() == 0)
            {
                //Debug.LogWarning("CalcClosestSegment".ColorTag(Color.red) + " call zero samplePoints");
                return;
            }

            foreach (IPenetrator penetrator in penetrators)
            {
                OrientedSegment penetration;

                if (!TryCalcPenetration(penetrator, out penetration)) { continue; }

                if (!penetration.HasLength) { continue; }

                stateSet.Add(new ShapeState(stateSet.Manipulator, penetration));
            }
        }

        protected abstract bool TryCalcPenetration(IPenetrator penetrator, out OrientedSegment penetration);

        #region IShapeContainer

        public GameObject Root { get { return RootGameObject; } }

        public IReadOnlyCollection<Collider> Colliders { get { return m_Colliders; } }

        #endregion
    }
}