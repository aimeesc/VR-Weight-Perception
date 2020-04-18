namespace exiii.Unity
{
    public abstract class SimpleManipulation<TManipulation> : ImmidiateManipulation<TManipulation>, ICycleManipulation<TManipulation>
        where TManipulation : class, ICycleManipulation<TManipulation>
    {
        public SimpleManipulation(IInteractorRoot controller, IManipulator<TManipulation> manipulator, IManipulable<TManipulation> manipulable)
            : base(controller, manipulator, manipulable) { }
    }
}