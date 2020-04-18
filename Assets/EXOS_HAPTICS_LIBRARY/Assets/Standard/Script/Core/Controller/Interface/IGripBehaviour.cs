using UnityEngine;
using System.Collections;

using Grippable = exiii.Unity.IManipulable<exiii.Unity.IGripManipulation>;

namespace exiii.Unity
{
    public interface IGripBehaviour
    {
        IExEventSet<Grippable> GripEvent { get; }

        Transform Center { get; }

        float Distance { get; }
    }
}
