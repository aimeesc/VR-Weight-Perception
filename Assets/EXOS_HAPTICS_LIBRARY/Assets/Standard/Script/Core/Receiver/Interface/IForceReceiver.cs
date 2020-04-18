using UnityEngine;

namespace exiii.Unity
{
    public interface IForceReceiver : IReceiver
    {
        // TODO : change to use this value from shapestate
        TransformState TransformState { get; }

        void AddForceRatio(Vector3 point, Vector3 force);

        void AddForceRatio(OrientedSegment segment);
    }
}