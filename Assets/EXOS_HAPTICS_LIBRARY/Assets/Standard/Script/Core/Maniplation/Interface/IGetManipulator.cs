namespace exiii.Unity
{
    public interface IGetManipulator
    {
        bool TryGetManipulator<TManipulator>(out TManipulator manipulator) where TManipulator : IManipulator;
    }
}
