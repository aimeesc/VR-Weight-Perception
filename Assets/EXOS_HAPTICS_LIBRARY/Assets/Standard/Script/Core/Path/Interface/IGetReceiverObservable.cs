using System;

namespace exiii.Unity
{
    public interface IGetReceiverObservable
    {
        bool TryGetReceiverObservable<TReceiver>(out IObservable<TReceiver> observable) where TReceiver : IReceiver;
    }
}