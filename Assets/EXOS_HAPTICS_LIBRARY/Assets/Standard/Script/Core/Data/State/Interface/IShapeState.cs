using UnityEngine;
using System.Collections;

namespace exiii.Unity
{
    public interface IShapeState : ITouchState
    {
        OrientedSegment ClosestSegment { get; }
    }
}
