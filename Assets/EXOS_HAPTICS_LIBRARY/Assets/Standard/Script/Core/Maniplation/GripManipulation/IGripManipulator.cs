namespace exiii.Unity
{
    public interface IGripManipulator : IManipulator<IGripManipulation>
    {
        IGripBehaviour GripController { get; }
    }
}