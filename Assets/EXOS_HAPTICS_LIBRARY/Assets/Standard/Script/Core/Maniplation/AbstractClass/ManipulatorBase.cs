using exiii.Extensions;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity
{
    public abstract class ManipulatorBase<TInterface, TClass> : InteractorNode, IManipulator<TInterface>
        where TInterface : class, IManipulation<TInterface>
        where TClass : ManipulationBase<TInterface>, TInterface
    {
        #region Inspector

        [Header("ManipulatorBase")]
        [SerializeField]
        private List<ManipulationFilterSetting> m_FilterSettings;

        public IReadOnlyCollection<ManipulationFilterSetting> FilterSettings { get { return m_FilterSettings; } }

        public bool IgnoreManipulatorFilter { get; set; } = true;
        public bool IgnoreManipulationFilter { get; set; } = false;

        [Header("DebugInspector")]
        [SerializeField]
        private GameObject[] m_ManipulationTargetObjects;

        #endregion Inspector

        #region Abstract

        public abstract EManipulationType ManipulationType { get; }

        protected abstract TClass GenerateManipulation(IManipulable<TInterface> manipulable);

        public abstract void CancelManipulation(IManipulable<TInterface> manipulable);

        #endregion Abstract

        private CompositeDisposable m_Disposables = new CompositeDisposable();

        public ICollection<IDisposable> Disposer => m_Disposables;

        public Dictionary<IManipulable<TInterface>, TClass> ManipulationTargets { get; } = new Dictionary<IManipulable<TInterface>, TClass>();

        public bool IsManipulating { get { return ManipulationTargets.Count > 0; } }

        protected override void Start()
        {
            base.Start();

            if (EHLDebug.DebugInspector)
            {
                SetupDebugInspector();
            }
        }

        private void SetupDebugInspector()
        {
            this.UpdateAsObservable()
                .Subscribe(_ => m_ManipulationTargetObjects = ManipulationTargets.Keys .Select(x => x.gameObject).ToArray());
        }

        public override void Initialize()
        {
            base.Initialize();

            if (m_Disposables.IsDisposed) { m_Disposables = new CompositeDisposable(); }

            if (InteractorRoot == null)
            {
                EHLDebug.Log($"{ExName}.InteractorRoot : Not found", this, "Path", ELogLevel.Overview);
                return;
            }
        }

        public override void Terminate()
        {
            if (m_Disposables != null && !m_Disposables.IsDisposed)
            {
                m_Disposables.Dispose();
            }
        }

        public void ResetManipulation()
        {
            ManipulationTargets.Values.Foreach(x => x.ResetManipulation());
        }

        private HashSet<Type> m_Subscrived = new HashSet<Type>();

        public IObservable<TReceiver> Observable<TReceiver>() where TReceiver : IReceiver
        {
            var subject = ReceiverSubjectCache<TReceiver>.GetSubject(this);

            var type = typeof(TReceiver);

            if (!m_Subscrived.Contains(type))
            {
                IObservable<TReceiver> observable;

                if (InteractorRoot != null && InteractorRoot.TryGetReceiverObservable(out observable))
                {
                    observable
                        .Where(x => IgnoreManipulatorFilter || CheckFilter(type))
                        .Subscribe(subject).AddTo(Disposer);

                    Disposer.Add(subject);

                    m_Subscrived.Add(type);

                    this.OnDisableAsObservable().Subscribe(_ => RemoveCache<TReceiver>(type));

                    EHLDebug.Log($"{ExName}.IPathController : {type.Name}", this, "Path");
                }
                else
                {
                    EHLDebug.Log($"{ExName}.IPathController : RootController not found", this, "Path");
                }
            }

            return subject;
        }

        private bool CheckFilter(Type type)
        {
            var filters = FilterSettings.CheckNull().Where(filter => filter.CheckPathType(type));

            if (filters.Count() == 0)
            {
                //Debug.Log($"[Manipulation] CheckFilter : {type.ToString()} : Zero");
                return true;
            }
            else
            {
                //Debug.Log($"[Manipulation] CheckFilter : {type.ToString()} : Filter");
                return filters.All(filter => filter.CheckFilter(InteractorRoot.ManipulatorState));
            }
        }

        /*
        public IDisposable Subscribe<TReceiver>(IObserver<TReceiver> observer) where TReceiver : IReceiver
        {
            return Observable<TReceiver>().Subscribe(observer);
        }
        */

        private void RemoveCache<TReceiver>(Type type) where TReceiver : IReceiver
        {
            ReceiverSubjectCache<TReceiver>.RemoveSubject(this);

            m_Subscrived.Remove(type);
        }
    }
}