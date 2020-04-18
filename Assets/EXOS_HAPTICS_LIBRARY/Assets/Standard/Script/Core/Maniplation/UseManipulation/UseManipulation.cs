namespace exiii.Unity
{
    public class UseManipulation : EventManipulation<IUseManipulation>, IUseManipulation
    {
        public override IUseManipulation Manipulation => this;

        public UseManipulation(IInteractorRoot controller, IManipulator<IUseManipulation> manipulator, IManipulable<IUseManipulation> manipulable)
            : base(controller, manipulator, manipulable)
        {
        }
    }
}