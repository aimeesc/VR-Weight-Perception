using exiii.Unity.Device;

namespace exiii.Unity.EXOS
{
    public enum EForceMode
    {
        Positive,
        Negative,
        Both,
    }

    public interface IExosForceReceiver : IReceiver
    {
        void AddDirectForceRatio(EAxisType axis, float forceRatio);

        void ChangeForceMode(EAxisType axis, EForceMode mode);
    }
}

