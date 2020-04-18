namespace exiii.Unity
{
    public class ManipulatorState : IManipulatorState
    {
        public ITouchManipulator Touch { get; private set; }

        public IGrabManipulator Grab { get; private set; }

        public IGripManipulator Grip { get; private set; }

        public IUseManipulator Use { get; private set; }

        public void Set(IManipulator manipulator)
        {
            switch (manipulator.ManipulationType)
            {
                case EManipulationType.Touch:
                    var touch = manipulator as ITouchManipulator;
                    if (touch != null) { Touch = touch; }
                    break;

                case EManipulationType.Grab:
                    var grab = manipulator as IGrabManipulator;
                    if (grab != null) { Grab = grab; }
                    break;

                case EManipulationType.Grip:
                    var grip = manipulator as IGripManipulator;
                    if (grip != null) { Grip = grip; }
                    break;

                case EManipulationType.Use:
                    var use = manipulator as IUseManipulator;
                    if (use != null) { Use = use; }
                    break;
            }
        }

        #region

        public bool TryGetManipulator(EManipulationType type, out IManipulator manipulator)
        {
            switch (type)
            {
                case EManipulationType.Touch:
                    if (Touch != null)
                    {
                        manipulator = Touch;
                        return true;
                    }
                    break;

                case EManipulationType.Grab:
                    if (Grab != null)
                    {
                        manipulator = Grab;
                        return true;
                    }
                    break;

                case EManipulationType.Grip:
                    if (Grip != null)
                    {
                        manipulator = Grip;
                        return true;
                    }
                    break;

                case EManipulationType.Use:
                    if (Use != null)
                    {
                        manipulator = Use;
                        return true;
                    }
                    break;
            }

            manipulator = null;
            return false;
        }

        #endregion
    }
}