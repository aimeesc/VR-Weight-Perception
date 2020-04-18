using System;

namespace exiii.Unity
{
    /// <summary>
    /// Interface for event handler
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IExEventSet<TData>
    {
        /// <summary>
        /// Observable called at the start of the event
        /// </summary>
        IObservable<TData> OnStart();

        /// <summary>
        /// Observable called during the continuation of the event
        /// </summary>
        IObservable<TData> OnStay();

        /// <summary>
        /// Observable called at the end of the event
        /// </summary>
        IObservable<TData> OnEnd();
    }
}