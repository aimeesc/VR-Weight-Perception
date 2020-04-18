namespace exiii.Unity
{
    public interface IPositionEffector : IEffector
    {
        void OnUpdatePosition(IPositionState state);

        void OnResetPosition();
    }
}