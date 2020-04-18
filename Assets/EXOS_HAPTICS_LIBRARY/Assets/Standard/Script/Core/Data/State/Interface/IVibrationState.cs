namespace exiii.Unity
{
    public interface IVibrationState : IState
    {
        IVibrationParameter VibrationParameter { get; }

        bool HasVibration { get; }
    }
}