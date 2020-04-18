namespace exiii.Unity
{
    public interface IGripState : IState
    {
        ISizeState SizeState { get; }
    }
}