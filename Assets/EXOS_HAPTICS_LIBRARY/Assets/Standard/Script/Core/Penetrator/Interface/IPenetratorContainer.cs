using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace exiii.Unity
{
	public interface IPenetratorContainer : IMonoBehaviour
	{
        IReadOnlyCollection<IPenetrator> Penetrators { get; }
    }
}
