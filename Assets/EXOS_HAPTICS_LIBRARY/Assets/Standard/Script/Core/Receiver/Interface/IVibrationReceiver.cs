namespace exiii.Unity
{
    public interface IVibrationReceiver : IReceiver
    {
        void AddVibration(IVibrationParameter parameter);
    }
}