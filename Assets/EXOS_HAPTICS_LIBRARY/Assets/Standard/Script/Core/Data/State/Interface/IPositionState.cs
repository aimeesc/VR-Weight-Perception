using UnityEngine;

namespace exiii.Unity
{
    public interface IPositionState : IState
    {
        Vector3 Position { get; }
        Quaternion Rotation { get; }
    }
}