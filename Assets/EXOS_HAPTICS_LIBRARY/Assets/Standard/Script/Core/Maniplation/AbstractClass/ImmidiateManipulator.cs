namespace exiii.Unity
{
    public abstract class ImmidiateManipulator<TInterface, TClass> : ColliderManipulator<TInterface, TClass>
        where TInterface : class, ICycleManipulation<TInterface>
        where TClass : ImmidiateManipulation<TInterface>, TInterface
    {
        #region ColliderManipulator

        public override void OnEnter(IManipulable<TInterface> component)
        {
            OnManipulationStart(component);
        }

        public override void OnExit(IManipulable<TInterface> component)
        {
            OnManipulationEnd(component);
        }

        public override void OnEachEnter(IManipulable<TInterface> component)
        {
            base.OnEachEnter(component);

            OnManipulationEnd(component);
            OnManipulationStart(component);
        }

        #endregion ColliderManipulator
    }
}