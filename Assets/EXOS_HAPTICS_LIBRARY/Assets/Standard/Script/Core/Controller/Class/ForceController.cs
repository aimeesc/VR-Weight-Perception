using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public class ForceController : ControllerBase, IForceReceiver, IForceState
    {
        public List<OrientedSegment> m_ForceList = new List<OrientedSegment>();
        public IReadOnlyCollection<OrientedSegment> ForceList => m_ForceList;

        public ForceController(TransformState state) : base(state) {  }

        // Reset
        public virtual void ResetVector()
        {
            m_ForceList.Clear();
        }

        // Force
        public virtual void AddForceRatio(Vector3 point, Vector3 force)
        {
            m_ForceList.Add(new OrientedSegment(point, point + force));
        }

        public virtual void AddForceRatio(OrientedSegment segment)
        {
            m_ForceList.Add(segment);
        }
    }
}