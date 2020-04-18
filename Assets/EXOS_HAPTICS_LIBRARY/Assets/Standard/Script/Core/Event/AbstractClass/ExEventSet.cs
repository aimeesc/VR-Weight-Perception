using exiii.Unity.Rx;
using System;

namespace exiii.Unity
{
    /// <summary>
    /// Base class of event handler
    /// </summary>
    /// <typeparam name="TData">Type of event</typeparam>
    public class ExEventSet<TData> : IExEventSet<TData>
    {
        /// <summary>
        /// Subject called at the start of the event
        /// </summary>
        public Subject<TData> Start { get; } = new Subject<TData>();

        /// <summary>
        /// Subject called during the continuation of the event
        /// </summary>
        public Subject<TData> Stay { get; } = new Subject<TData>();

        /// <summary>
        /// Subject called at the end of the event
        /// </summary>
        public Subject<TData> End { get; } = new Subject<TData>();

        /// <summary>
        /// Observable called at the start of the event
        /// </summary>
        public IObservable<TData> OnStart() => Start;

        /// <summary>
        /// Observable called during the continuation of the event
        /// </summary>
        public IObservable<TData> OnStay() => Stay;

        /// <summary>
        /// Observable called at the end of the event
        /// </summary>
        public IObservable<TData> OnEnd() => End;
    }
}