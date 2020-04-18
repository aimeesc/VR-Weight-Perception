using UnityEngine;

namespace exiii.Unity
{
    //TODO: Correspond to rotation
    public class PositionController : ControllerBase, IPositionReceiver, IPositionState
    {
        public PositionController(TransformState state) : base(state)
        {            
        }

        public void ResetVector()
        {
            Position = Vector3.zero;
        }

        #region IPositionReceiver

        public void AddPositionRatio(Vector3 position)
        {
            Position += position;
        }

        #endregion IPositionReceiver

        #region IPositionResult

        public Vector3 Position { get; private set; }

        public Quaternion Rotation { get; private set; }

        #endregion IPositionResult
    }
}