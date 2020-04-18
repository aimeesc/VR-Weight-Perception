namespace exiii.Unity
{
    public abstract class ColliderManipulation<TManipulation> : CycleManipulation<TManipulation>, ICycleManipulation<TManipulation>
        where TManipulation : class, ICycleManipulation<TManipulation>
    {
        public ColliderManipulation(IInteractorRoot controller, IManipulator<TManipulation> manipulator, IManipulable<TManipulation> manipulable)
            : base(controller, manipulator, manipulable) { }
    }
}