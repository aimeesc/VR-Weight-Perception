using exiii.Unity.Rx;
using System;
using System.Collections.Generic;

namespace exiii.Unity
{
    public interface IManipulation : IDisposable
    {
        EManipulationType ManipulationType { get; }

        IInteractorRoot InteractorRoot { get; }

        IObservable<Unit> OnDisposing();

        ICollection<IDisposable> Disposer { get; }

        bool IsDone { get; }
    }

    public interface IManipulation<TManipulation> : IManipulation, IReceiverObservableHolder
        where TManipulation : IManipulation<TManipulation>
    {
        IManipulator<TManipulation> Manipulator { get; }

        void CancelManipulation(IManipulable<TManipulation> manipulable);
    }
}