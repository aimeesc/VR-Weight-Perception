using exiii.Extensions;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace exiii.Unity
{
    public abstract class CycleManipulator<TInterface, TClass> : ManipulatorBase<TInterface, TClass>
        where TInterface : class, ICycleManipulation<TInterface>
        where TClass : CycleManipulation<TInterface>, TInterface
    {
        public override void CancelManipulation(IManipulable<TInterface> manipulable)
        {
            DoCancel(manipulable);
        }

        private void DoCancel(IManipulable<TInterface> manipulable)
        {
            TClass manipulation;
            if (!ManipulationTargets.TryGetValue(manipulable, out manipulation)) { return; }

            if (!manipulation.IsDone)
            {
                try
                {
                    manipulation.ManipulateEnd.OnNext(this);
                }
                catch (Exception e)
                {
                    EHLDebug.LogError($"[Exception] {ExName}.DoCancel : {e}", this, "Manipulation");
                    throw;
                }
                finally
                {
                    manipulation.Dispose();

                    ManipulationTargets.Remove(manipulable);
                }
            }
            else
            {
                ManipulationTargets.Remove(manipulable);
            }

            EHLDebug.Log($"{ExName}.DoCancel", this, "Manipulation");
        }

        protected virtual void OnManipulationStart(IEnumerable<IManipulable<TInterface>> enumerable)
        {
            enumerable.Foreach(x => OnManipulationStart(x));
        }

        protected virtual void OnManipulationStart(IManipulable<TInterface> manipulable)
        {
            TClass manipulation;

            if (ManipulationTargets.ContainsKey(manipulable))
            {
                manipulation = ManipulationTargets[manipulable];

                if (manipulation.IsDone)
                {
                    EHLDebug.LogWarning($"{ExName}.CycleManipulationStart : {manipulable} is already finished", this, "Manipulation");

                    DoCancel(manipulable);
                    return;
                }

                manipulation.ManipulateStart.OnNext(this);
                return;
            }

            manipulation = GenerateManipulation(manipulable);

            if (manipulable.TryStartManipulation(manipulation))
            {
                EHLDebug.Log($"{ExName}.CycleManipulationStart : {manipulable.gameObject}", this, "Manipulation");

                ManipulationTargets.Add(manipulable, manipulation);

                manipulation.ManipulateStart.OnNext(this);

                this.UpdateAsObservable()
                    .ObserveOnMainThread()
                    .Subscribe(_ => ManipulateUpdate(manipulation))
                    .AddTo(manipulation.Disposer);

                this.FixedUpdateAsObservable()
                    .ObserveOnMainThread()
                    .Subscribe(_ => ManipulateFixedUpdate(manipulation))
                    .AddTo(manipulation.Disposer);

                manipulable
                    .gameObject
                    .OnDisableAsObservable()
                    .Subscribe(_ => CancelManipulation(manipulable))
                    .AddTo(manipulation.Disposer);
            }
            else
            {
                EHLDebug.Log($"{ExName}.CycleManipulationFailed : {manipulable.gameObject}", this, "Manipulation");
            }
        }

        private void ManipulateUpdate(TClass manipulation)
        {
            if (manipulation.IsDone) { return; }

            manipulation.ResetManipulation();
            manipulation.ManipulateUpdate.OnNext(this);
        }

        private void ManipulateFixedUpdate(TClass manipulation)
        {
            if (manipulation.IsDone) { return; }

            manipulation.ResetManipulation();
            manipulation.ManipulateFixedUpdate.OnNext(this);
        }

        protected virtual void OnManipulationEnd(IManipulable<TInterface> manipulable)
        {
            TClass manipulation;

            if (!ManipulationTargets.TryGetValue(manipulable, out manipulation)) { return; }

            if (manipulation.IsManualManipulation)
            {
                EHLDebug.Log($"{ExName}.IsManualManipulation  : {manipulable.gameObject}", this, "Manipulation");
                return;
            }

            DoCancel(manipulable);
        }

        protected virtual void OnManipulationEnd(IEnumerable<IManipulable<TInterface>> enumerable)
        {
            // HACK: need optimization
            foreach (var manipulable in enumerable.ToArray()) { OnManipulationEnd(manipulable); }
        }

        protected virtual void OnManipulationEnd()
        {
            foreach (var manipulable in ManipulationTargets.Keys) { OnManipulationEnd(manipulable); }
        }
    }
}