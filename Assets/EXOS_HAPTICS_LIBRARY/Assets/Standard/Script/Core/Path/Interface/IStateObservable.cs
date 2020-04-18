using System;

namespace exiii.Unity
{
    public interface IStateObservable : IMonoBehaviour
    {
        IObservable<TState> Observable<TState>() where TState : IState;
    }

    public interface IStateObservable<TState> : IObservable<TState>
        where TState : IState
    {
    }
}