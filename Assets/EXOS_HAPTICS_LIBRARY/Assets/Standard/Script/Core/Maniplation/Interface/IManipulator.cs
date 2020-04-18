using System;
using System.Collections.Generic;

namespace exiii.Unity
{
    public interface IManipulator : IMonoBehaviour
    {
        EManipulationType ManipulationType { get; }

        IReadOnlyCollection<ManipulationFilterSetting> FilterSettings { get; }

        ICollection<IDisposable> Disposer { get; }

        bool IsManipulating { get;}

        bool IgnoreManipulatorFilter { get; set; }
        bool IgnoreManipulationFilter { get; set; }

        void ResetManipulation();
    }

    public interface IManipulator<TManipuration> : IManipulator , IReceiverObservable
        where TManipuration : IManipulation<TManipuration>
    {
        void CancelManipulation(IManipulable<TManipuration> manipulable);
    }
}