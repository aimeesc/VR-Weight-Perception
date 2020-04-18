using System;

namespace exiii.Unity
{
    public interface IGetStateObservable
    {
        bool TryGetStateObservable<TState>(out IObservable<TState> observable) where TState : IState;
    }
}