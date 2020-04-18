using System;

namespace exiii.Unity
{
    /// <summary>
    /// Interface to detect contact of root object
    /// </summary>
    public interface IDetectable : INodeScript
    {
        /// <summary>
        /// Function called at the start of contact
        /// </summary>
        IObservable<IDetectable> OnEnter();

        /// <summary>
        /// Function called at the end of contact
        /// </summary>
        IObservable<IDetectable> OnExit();

        /// <summary>
        /// Function called at the start of each contact 
        /// </summary>
        IObservable<IDetectable> OnEachEnter();

        /// <summary>
        /// Function called at the end of each contact
        /// </summary>
        IObservable<IDetectable> OnEachExit();

        IDisposable Subscribe(IDetector detector);
    }

    /// <summary>
    /// Interface to detect contact of root object
    /// </summary>
    public interface IDetectable<TComponent> : INodeScript
    {
        /// <summary>
        /// Function called at the start of contact
        /// </summary>
        IObservable<TComponent> OnEnter();

        /// <summary>
        /// Function called at the end of contact
        /// </summary>
        IObservable<TComponent> OnExit();

        /// <summary>
        /// Function called at the start of each contact 
        /// </summary>
        IObservable<TComponent> OnEachEnter();

        /// <summary>
        /// Function called at the end of each contact
        /// </summary>
        IObservable<TComponent> OnEachExit();

        IDisposable Subscribe(IDetector<TComponent> detector);
    }
}