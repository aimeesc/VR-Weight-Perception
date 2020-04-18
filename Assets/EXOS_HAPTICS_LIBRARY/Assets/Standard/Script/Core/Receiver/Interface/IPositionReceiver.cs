//using exiii.Unity.Device;
using UnityEngine;

namespace exiii.Unity
{
    public interface IPositionReceiver : IReceiver
    {
        //IExosDevice Device { get; }

        void AddPositionRatio(Vector3 position);
    }
}