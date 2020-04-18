namespace exiii.Unity
{
    /// <summary>
    /// Determine a Grippable object in contact with Gripper's Thamb and Index
    /// Designate Grippable event type as generic
    /// </summary>
    /// <typeparam name="TManipulation">Grippable event type</typeparam>
    public abstract class ManipulableDetector<TManipulation> : DetectedHolder<IManipulable<TManipulation>>
        where TManipulation : IManipulation<TManipulation>
    {
    }
}