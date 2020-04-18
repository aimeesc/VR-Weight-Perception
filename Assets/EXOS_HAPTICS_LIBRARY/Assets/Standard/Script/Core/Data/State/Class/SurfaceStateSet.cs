using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace exiii.Unity
{
	public class SurfaceStateSet : TouchState, ISurfaceStateSet
	{
        private List<ISurfaceState> m_Collection = new List<ISurfaceState>();

        public IReadOnlyCollection<ISurfaceState> Collection { get { return m_Collection; } }

        public bool Enabled { get { return m_Collection.Count > 0; } }

        public SurfaceStateSet(ITouchManipulator manipulator) : base(manipulator)
        {
        }

        public void Add(ISurfaceState state)
        {
            m_Collection.Add(state);
        }

        public void Clear()
        {
            m_Collection.Clear();
        }
    }
}
