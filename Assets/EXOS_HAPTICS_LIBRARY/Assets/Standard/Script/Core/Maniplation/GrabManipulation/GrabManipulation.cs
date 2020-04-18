namespace exiii.Unity
{
    public class GrabManipulation : EventManipulation<IGrabManipulation>, IGrabManipulation
    {
        public override IGrabManipulation Manipulation => this;

        public GrabManipulation(IInteractorRoot controller, IGrabManipulator manipulator, IManipulable<IGrabManipulation> manipulable)
            : base(controller, manipulator, manipulable)
        {
        }
    }
}