using UnityEngine;

namespace exiii.Unity
{
    public interface ISizeState : IState
    {
        Transform Center { get; }

        float SizeRatio { get; }
    }
}