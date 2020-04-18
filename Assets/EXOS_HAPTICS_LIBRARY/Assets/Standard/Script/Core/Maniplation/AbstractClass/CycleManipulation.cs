using exiii.Extensions;
using exiii.Unity.Rx;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public abstract class CycleManipulation<TManipulation> : ManipulationBase<TManipulation>, ICycleManipulation<TManipulation>
        where TManipulation : class, ICycleManipulation<TManipulation>
    {
        public bool IsManualManipulation { get; set; } = false;

        public CycleManipulation(IInteractorRoot controller, IManipulator<TManipulation> manipulator, IManipulable<TManipulation> manipulable)
            : base(controller, manipulator, manipulable)
        {
            Disposer.Add(ManipulateStart);
            Disposer.Add(ManipulateUpdate);
            Disposer.Add(ManipulateFixedUpdate);
            Disposer.Add(ManipulateEnd);
        }

        //public IManipulable<TInterface> Manipulable;

        public void RegisterCycle(ICollection<TManipulation> manipulations)
        {
            if (manipulations == null) { return; }

            ManipulateStart
                .Where(_ => !manipulations.Contains(Manipulation))
                .Subscribe(_ => manipulations.Add(Manipulation));

            ManipulateEnd
                .Where(_ => manipulations.Contains(Manipulation))
                .Subscribe(_ => manipulations.Remove(Manipulation));
        }

        public void AddCycleScripts(IEnumerable<ICycleScript> scripts)
        {
            if (scripts == null) { return; }

            scripts.Foreach(script => ManipulateStart.Subscribe(_ => script.OnStart()));
            scripts.Foreach(script => ManipulateUpdate.Subscribe(_ => script.OnUpdate()));
            scripts.Foreach(script => ManipulateFixedUpdate.Subscribe(_ => script.OnFixedUpdate()));
            scripts.Foreach(script => ManipulateEnd.Subscribe(_ => script.OnEnd()));
        }

        public void AddCycleScripts(IEnumerable<ICycleScript<TManipulation>> scripts)
        {
            if (scripts == null) { return; }

            scripts.Foreach(script => ManipulateStart.Subscribe(_ => script.OnStart(Manipulation)));
            scripts.Foreach(script => ManipulateUpdate.Subscribe(_ => script.OnUpdate(Manipulation)));
            scripts.Foreach(script => ManipulateFixedUpdate.Subscribe(_ => script.OnFixedUpdate(Manipulation)));
            scripts.Foreach(script => ManipulateEnd.Subscribe(_ => script.OnEnd(Manipulation)));
        }

        public void AddCycleScripts(IEnumerable<ICycleScript<GameObject>> scripts)
        {
            if (scripts == null) { return; }

            scripts.Foreach(script => ManipulateStart.Subscribe(_ => script.OnStart(Manipulator.gameObject)));
            scripts.Foreach(script => ManipulateUpdate.Subscribe(_ => script.OnUpdate(Manipulator.gameObject)));
            scripts.Foreach(script => ManipulateFixedUpdate.Subscribe(_ => script.OnFixedUpdate(Manipulator.gameObject)));
            scripts.Foreach(script => ManipulateEnd.Subscribe(_ => script.OnEnd(Manipulator.gameObject)));
        }

        public Subject<IManipulator<TManipulation>> ManipulateStart = new Subject<IManipulator<TManipulation>>();

        public IObservable<IManipulator<TManipulation>> OnManipulateStart()
        {
            return ManipulateStart;
        }

        public Subject<IManipulator<TManipulation>> ManipulateUpdate = new Subject<IManipulator<TManipulation>>();

        public IObservable<IManipulator<TManipulation>> OnManipulateUpdate()
        {
            return ManipulateUpdate;
        }

        public Subject<IManipulator<TManipulation>> ManipulateFixedUpdate = new Subject<IManipulator<TManipulation>>();

        public IObservable<IManipulator<TManipulation>> OnManipulateFixedUpdate()
        {
            return ManipulateFixedUpdate;
        }

        public Subject<IManipulator<TManipulation>> ManipulateEnd = new Subject<IManipulator<TManipulation>>();

        public IObservable<IManipulator<TManipulation>> OnManipulateEnd()
        {
            return ManipulateEnd;
        }
    }
}