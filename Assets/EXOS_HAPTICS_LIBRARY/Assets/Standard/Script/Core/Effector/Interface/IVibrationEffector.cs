namespace exiii.Unity
{
    public interface IVibrationEffector : IEffector
    {
        void OnVibrate(IVibrationState state);
    }
}