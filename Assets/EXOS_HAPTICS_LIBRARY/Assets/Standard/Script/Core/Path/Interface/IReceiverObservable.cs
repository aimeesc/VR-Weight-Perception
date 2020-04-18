using System;

namespace exiii.Unity
{
    public interface IReceiverObservable : IMonoBehaviour
    {
        IObservable<TReceiver> Observable<TReceiver>() where TReceiver : IReceiver;
    }

    public interface IReceiverObservable<TReceiver> : IObservable<TReceiver>
        where TReceiver : IReceiver
    {

    }
}