using System.Collections.Generic;

namespace exiii.Unity
{
    public interface IForceState
    {
        IReadOnlyCollection<OrientedSegment> ForceList { get; }
    }
}