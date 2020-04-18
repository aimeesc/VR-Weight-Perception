namespace exiii.Unity
{
    public interface IGenerator { }

    public interface IGenerator<TReceiver, TState> : IGenerator
        where TReceiver : IReceiver
        where TState : IState
    {
        void OnGenerate(TReceiver receiver, TState state);
    }
}