namespace exiii.Unity
{
    public interface IManipulationState
    {
        /*
        bool IsTouched { get; }
        bool IsGrabbed { get; }
        bool IsGripped { get; }
        bool IsUsed { get; }
        */

        bool IsManipulated(EManipulationType type);

        bool IsManipulatedBy(IManipulator manipulator);
    }
}