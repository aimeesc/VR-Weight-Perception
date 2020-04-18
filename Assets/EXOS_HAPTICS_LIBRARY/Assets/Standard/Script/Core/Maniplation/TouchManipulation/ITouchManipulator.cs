namespace exiii.Unity
{
    public interface ITouchManipulator : IManipulator<ITouchManipulation>
    {
        IPenetratorContainer PenetratorContainer { get; }

        TouchForceParameter TouchForceParameter { get; }

        bool PenetratorIsDefault { get; }

        void ChangePenetrator(IPenetratorContainer container);

        void RestorePenetrator();
    }
}