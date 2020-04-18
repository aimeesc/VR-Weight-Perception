using exiii.Unity.Rx;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using exiii.Extensions;

namespace exiii.Unity
{
    public abstract class ManipulationBase<TManipulation> : IManipulation<TManipulation>
        where TManipulation : class, IManipulation<TManipulation>
    {
        public abstract TManipulation Manipulation { get; }

        public IInteractorRoot InteractorRoot { get; }

        public IManipulator<TManipulation> Manipulator { get; }
        public IManipulable<TManipulation> Manipulable { get; }
        public EManipulationType ManipulationType { get; }

        private CompositeDisposable m_Disposables = new CompositeDisposable();
        public ICollection<IDisposable> Disposer => m_Disposables;

        public bool IsDone => disposedValue;

        public ManipulationBase(IInteractorRoot root, IManipulator<TManipulation> manipulator, IManipulable<TManipulation> manipulable)
        {
            InteractorRoot = root;

            Manipulator = manipulator;
            Manipulable = manipulable;

            ManipulationType = Manipulator.ManipulationType;

            Disposer.Add(m_Disposing);
        }

        public virtual void ResetManipulation() { }

        public virtual void CancelManipulation(IManipulable<TManipulation> manipulable)
        {
            if (manipulable == null) { return; }

            Manipulator.CancelManipulation(manipulable);
        }

        /*
        public void AddTo(GameObject obj)
        {
            m_Disposables.AddTo(obj);
        }
        */

        private Subject<Unit> m_Disposing = new Subject<Unit>();

        public IObservable<Unit> OnDisposing()
        {
            return m_Disposing;
        }

        #region IManipulation

        private HashSet<Type> m_Subscrived = new HashSet<Type>();
        
        public IObservable<TReceiver> GetObservable<TReceiver>()
            where TReceiver : IReceiver
        {
            var subject = ReceiverSubjectCache<TReceiver>.GetSubject(this);

            var type = typeof(TReceiver);

            if (!m_Subscrived.Contains(type))
            {
                Manipulator.Observable<TReceiver>()
                    .Where(x => Manipulator.IgnoreManipulationFilter || CheckFilter(type))
                    .Subscribe(subject).AddTo(Disposer);

                Disposer.Add(subject);

                OnDisposing().Subscribe(_ => RemoveCache<TReceiver>(type));

                m_Subscrived.Add(type);
            }

            return subject;
        }

        private bool CheckFilter(Type type)
        {
            var filters = Manipulator.FilterSettings.CheckNull().Where(filter => filter.CheckPathType(type));

            if (filters.Count() == 0)
            {
                //Debug.Log($"[Manipulation] CheckFilter : {type.ToString()} : Zero");
                return true;
            }
            else
            {
                //Debug.Log($"[Manipulation] CheckFilter : {type.ToString()} : Filter");
                return filters.All(filter => filter.CheckFilter(InteractorRoot.ManipulatorState, Manipulable.ManipulationState));
            }
        }

        private void RemoveCache<TReceiver>(Type type) where TReceiver : IReceiver
        {
            ReceiverSubjectCache<TReceiver>.RemoveSubject(this);

            m_Subscrived.Remove(type);
        }

        #endregion IManipulation

        #region IDisposable

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    m_Disposing.OnNext(Unit.Default);
                    m_Disposables.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable
    }
}