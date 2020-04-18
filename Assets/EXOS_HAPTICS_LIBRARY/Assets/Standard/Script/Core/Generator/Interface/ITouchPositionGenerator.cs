namespace exiii.Unity
{
    /// <summary>
    /// An interface for acquiring the relative position between the physical operation model and the display model
    /// </summary>
    public interface ITouchPositionGenerator : IGenerator<IPositionReceiver, IShapeStateSet>
    {        
    }
}