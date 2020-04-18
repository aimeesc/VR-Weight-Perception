using System;

namespace exiii.Unity
{
    public interface IReceiverObservableHolder
    {
        // void GetObservable<TReceiver>(out IObservable<TReceiver> observable) where TReceiver : IReceiver;
        IObservable<TReceiver> GetObservable<TReceiver>() where TReceiver : IReceiver;
    }

    /*
    public interface IReceiverPathHolder<TReceiver>
        where TReceiver : IReceiver
    {
        // void GetObservable(out IObservable<TReceiver> observable);
        IObservable<TReceiver> GetObservable();
    }
    */
}