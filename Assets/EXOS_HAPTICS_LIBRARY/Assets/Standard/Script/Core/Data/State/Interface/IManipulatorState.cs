namespace exiii.Unity
{
    public interface IManipulatorState
    {
        bool TryGetManipulator(EManipulationType type, out IManipulator manipulator);
    }
}