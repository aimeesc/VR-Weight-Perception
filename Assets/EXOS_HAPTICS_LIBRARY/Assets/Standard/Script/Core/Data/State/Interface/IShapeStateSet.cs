using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace exiii.Unity
{
	public interface IShapeStateSet : ITouchState
	{
        OrientedSegment SummarizedOutput { get; }

        IReadOnlyCollection<IShapeState> Collection { get; }
    }
}
