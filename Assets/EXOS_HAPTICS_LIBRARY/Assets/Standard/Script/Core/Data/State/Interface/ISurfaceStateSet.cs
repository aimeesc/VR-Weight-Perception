using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace exiii.Unity
{
	public interface ISurfaceStateSet : IState
    {
        IReadOnlyCollection<ISurfaceState> Collection { get; }
    }
}
