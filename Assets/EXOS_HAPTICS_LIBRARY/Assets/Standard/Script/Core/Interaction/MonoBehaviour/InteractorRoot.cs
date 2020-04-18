using exiii.Extensions;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

#pragma warning disable 414

namespace exiii.Unity
{
    [RequireComponent(typeof(MultiCollider))]
    public class InteractorRoot : RootScript, IInteractorRoot
    {
        #region Inspector

        [Header(nameof(InteractorRoot))]
        [SerializeField]
        [FormerlySerializedAs("m_TrackerParameter")]
        [FormerlySerializedAs("m_TrackingParameter")]
        private TrackingSettings m_TrackingSettings = new TrackingSettings();

        public TrackingSettings TrackingSettings { get { return m_TrackingSettings; } }

        [SerializeField]
        private bool m_PositionUpdateOnUpdate = true;

        [Header("Shortcut")]
        [SerializeField, Unchangeable]
        private GameObject[] m_ManipulatorInChildren;

        [SerializeField, Unchangeable]
        private GameObject[] m_PrefabInChildren;

        #endregion Inspector

        private Subject<Transform> m_PositionUpdate = new Subject<Transform>();

        public IObservable<Transform> OnPositionUpdate() { return m_PositionUpdate; }

        protected override void Start()
        {
            base.Start();

            this.UpdateAsObservable().Where(_ => m_PositionUpdateOnUpdate).Subscribe(_ => m_PositionUpdate.OnNext(transform));

            GetComponentsInChildren<IManipulator>().Foreach(x => m_ManipulatorState.Set(x));

            if (EHLDebug.DebugInspector)
            {
                SetupDebugInspector();
            }
        }

        private void SetupDebugInspector()
        {
            m_ManipulatorInChildren = GetComponentsInChildren<IManipulator>().Select(x => x.gameObject).Distinct().ToArray();

            m_PrefabInChildren = GetComponentsInChildren<IExosPrefab>().Select(x => x.gameObject).Distinct().ToArray();
        }

        public void SetTracker(ITracker tracker)
        {
            tracker.OnPositionUpdate().Subscribe(m_PositionUpdate).AddTo(tracker.gameObject);

            m_PositionUpdateOnUpdate = false;
        }

        #region IInteractorRoot

        private ManipulatorState m_ManipulatorState = new ManipulatorState();

        public IManipulatorState ManipulatorState { get { return m_ManipulatorState; } }

        public bool TryGetManipulator<TManipulator>(out TManipulator manipulator) where TManipulator : IManipulator
        {
            manipulator = GetComponentInChildren<TManipulator>();

            return manipulator != null;
        }

        public bool TryGetReceiverObservable<TReceiver>(out IObservable<TReceiver> observable) where TReceiver : IReceiver
        {
            observable = GetComponentInChildren<IReceiverObservable<TReceiver>>();

            return observable != null;
        }

        public bool TryGetStateObservable<TState>(out IObservable<TState> observable) where TState : IState
        {
            observable = GetComponentInChildren<IStateObservable<TState>>();

            return observable != null;
        }

        #endregion IRootController
    }
}