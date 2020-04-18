namespace exiii.Unity
{
    public class ControllerBase : IReceiver, IState
    {
        public TransformState TransformState { get; }

        public ControllerBase(TransformState state)
        {
            TransformState = state;
        }
    }
}