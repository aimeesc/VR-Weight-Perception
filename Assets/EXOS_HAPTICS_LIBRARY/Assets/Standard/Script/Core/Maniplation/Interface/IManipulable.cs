namespace exiii.Unity
{
    public interface IManipulable : IMonoBehaviour
    {
        IManipulationState ManipulationState { get; }
    }

    public interface IManipulable<TManipulation> : IManipulable where TManipulation : IManipulation<TManipulation>
    {
        bool TryStartManipulation(TManipulation manipulation);
    }
}