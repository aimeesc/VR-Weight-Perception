namespace exiii.Unity
{
    public abstract class EventManipulation<TManipulation> : ColliderManipulation<TManipulation>
        where TManipulation : class, ICycleManipulation<TManipulation>
    {
        public EventManipulation(IInteractorRoot controller, IManipulator<TManipulation> manipulator, IManipulable<TManipulation> manipulable)
            : base(controller, manipulator, manipulable) { }
    }
}