namespace exiii.Unity
{
    public interface IGripManipulation : ICycleManipulation<IGripManipulation>
    {
        IGripState GripState { get; }

        bool TryUpdateGripState();
    }
}