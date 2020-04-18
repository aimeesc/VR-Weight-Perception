using System;
using UnityEngine;

namespace exiii.Unity
{
    public interface IInteractorRoot : IRootScript,
        IGetManipulator,
        IGetReceiverObservable,
        IGetStateObservable
    {
        IManipulatorState ManipulatorState { get; }

        TrackingSettings TrackingSettings { get; }

        void SetTracker(ITracker tracker);

        IObservable<Transform> OnPositionUpdate();
    }
}