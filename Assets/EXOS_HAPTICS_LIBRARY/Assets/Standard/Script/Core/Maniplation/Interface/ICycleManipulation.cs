using System;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public interface ICycleManipulation<TManipulation> : IManipulation<TManipulation>
        where TManipulation : ICycleManipulation<TManipulation>
    {
        bool IsManualManipulation { get; set; }

        void RegisterCycle(ICollection<TManipulation> manipulations);

        void AddCycleScripts(IEnumerable<ICycleScript> cycles);

        void AddCycleScripts(IEnumerable<ICycleScript<GameObject>> cycles);

        void AddCycleScripts(IEnumerable<ICycleScript<TManipulation>> cycles);

        //void AddDisposable(IDisposable disposable);

        IObservable<IManipulator<TManipulation>> OnManipulateStart();

        IObservable<IManipulator<TManipulation>> OnManipulateUpdate();

        IObservable<IManipulator<TManipulation>> OnManipulateFixedUpdate();

        IObservable<IManipulator<TManipulation>> OnManipulateEnd();
    }
}