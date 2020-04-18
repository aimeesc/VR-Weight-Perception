namespace exiii.Unity
{
    public class GripManipulation : EventManipulation<IGripManipulation>, IGripManipulation
    {
        private GripState m_GripState;

        public IGripState GripState => m_GripState;

        public override IGripManipulation Manipulation => this;

        private IGripManipulator m_Manipulator;

        private bool m_GripStateUpdated = false;

        public GripManipulation(IInteractorRoot controller, IGripManipulator manipulator, IManipulable<IGripManipulation> manipulable)
            : base(controller, manipulator, manipulable)
        {
            m_Manipulator = manipulator;

            m_GripState = new GripState(m_Manipulator.GripController.Center, m_Manipulator.GripController.Distance);
        }

        public override void ResetManipulation()
        {
            m_GripStateUpdated = false;
        }

        public bool TryUpdateGripState()
        {
            if (m_GripStateUpdated) { return true; }

            m_GripStateUpdated = true;

            m_GripState.Distance = m_Manipulator.GripController.Distance;

            return true;
        }
    }
}