namespace exiii.Unity
{
    public abstract class ImmidiateManipulation<TManipulation> : ColliderManipulation<TManipulation>
        where TManipulation : class, ICycleManipulation<TManipulation>
    {
        public ImmidiateManipulation(IInteractorRoot controller, IManipulator<TManipulation> manipulator, IManipulable<TManipulation> manipulable)
            : base(controller, manipulator, manipulable) { }
    }
}