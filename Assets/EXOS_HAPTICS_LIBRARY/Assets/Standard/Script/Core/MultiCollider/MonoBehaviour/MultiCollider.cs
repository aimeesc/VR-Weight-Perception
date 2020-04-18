using exiii.Extensions;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity
{
    public class MultiCollider : NodeScript, IDetectable
    {
        #region Inspector

        [Header(nameof(MultiCollider))]

        [SerializeField]
        private EDetectionType m_DetectionType = EDetectionType.Any;

        public EDetectionType DetectionType
        {
            get { return m_DetectionType; }
            set { m_DetectionType = value; }
        }

        [SerializeField]
        private Collider[] m_Colliders;

        [SerializeField]
        private bool m_AutoSearchColliders = true;

        [SerializeField]
        private EListType m_ApplyListType = EListType.BlackOR;

        [SerializeField]
        private ExTagContainer[] m_ApplyList;

        public IReadOnlyCollection<IExTag> ApplyList { get { return m_ApplyList; } }

        [Header("DebugInspector")]
        [SerializeField]
        private ColliderState[] m_Children;

        [SerializeField]
        private MultiCollider[] m_Oppsites;

        [SerializeField]
        private bool m_ConnectToOpposite = false;

        #endregion Inspector

        private ReactiveDictionary<ColliderState, IDisposable> m_ChildrenDic = new ReactiveDictionary<ColliderState, IDisposable>();

        public IReadOnlyCollection<ColliderState> Children => m_ChildrenDic.Keys;

        private ReactiveCollection<MultiCollider> m_OppositesList = new ReactiveCollection<MultiCollider>();

        public IReadOnlyCollection<MultiCollider> Opposites => m_OppositesList;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_AutoSearchColliders) { SearchColliders(transform); }

            if (m_Colliders != null)
            {
                var group = m_Colliders.CheckNull().GroupBy(x => x.gameObject).Where(x => x.Count() > 1);

                group.Foreach(x => Debug.LogWarning($"{nameof(MultiCollider)} disallow multiple colliders on 1 GameObjct : {x.Key} has {x.Count()} colliders", x.Key));
            }
        }

        protected override void Start()
        {
            base.Start();

            if (EHLDebug.DebugDrawGL)
            {
                SetupDebugDrawGL();
            }

            if (EHLDebug.DebugInspector)
            {
                SetupDebugInspector();
            }
        }

        private void SetupDebugDrawGL()
        {
            if (!m_ConnectToOpposite) { return; }

            this.UpdateAsObservable()
                .Subscribe(_ => Opposites.Foreach(x => LineDrawerGL.DrawLine(transform.position, x.transform.position)));
        }

        private void SetupDebugInspector()
        {
            m_Children = Children.ToArray();
            m_Oppsites = m_OppositesList.ToArray();

            m_ChildrenDic
                .ObserveCountChanged()
                .Subscribe(_ => m_Children = Children.ToArray());

            m_OppositesList
                .ObserveCountChanged()
                .Subscribe(_ => m_Oppsites = Opposites.ToArray());
        }

        public override void Initialize()
        {
            base.Initialize();

            Build();
        }

        public void SearchColliders(Transform trans)
        {
            if (trans == null) { return; }

            var colliders = trans.GetComponentsInChildren<Collider>();

            if (colliders == null || colliders.Count() == 0) { return; }

            m_Colliders = colliders
                .Where(x => CheckApplyList(x))
                .ToArray();
        }

        private bool CheckApplyList(Collider collider)
        {
            var tags = collider.GetComponents<IExTag>();

            switch (m_ApplyListType)
            {
                case EListType.WhiteOR:
                    foreach (var tag in tags)
                    {
                        if (tag != null && ApplyList.Contains(tag, ExTagComparer.Instance)) { return true; }
                    }

                    return false;

                case EListType.BlackOR:
                    foreach (var tag in tags)
                    {
                        if (tag != null && ApplyList.Contains(tag, ExTagComparer.Instance)) { return false; }
                    }

                    return true;

                default:
                    return true;
            }
        }

        public void Build()
        {
            if (m_AutoSearchColliders) { SearchColliders(transform); }

            if (m_Colliders == null) { return; }

            m_Colliders.Foreach(x => AddChildren(x));
        }

        public void AddChildren(Collider collider)
        {
            if (collider == null) { return; }

            var contacts = collider.GetComponents<ColliderState>();

            ColliderState state;

            if (contacts == null || contacts.All(c => c.Collider != collider))
            {
                state = collider.gameObject.AddComponent<ColliderState>();

                state.Collider = collider;
            }
            else
            {
                state = contacts.Where(c => c.Collider = collider).First();
            }

            state.AddRootUnique(this);

            if (!m_ChildrenDic.ContainsKey(state))
            {
                var disposable = this.OnDisableAsObservable()
                    .Subscribe(_ => RemoveChildren(state))
                    .AddTo(this);

                m_ChildrenDic.Add(state, disposable);
            }
        }

        public void RemoveChildren(ColliderState state)
        {
            state.RemoveRoot(this);

            IDisposable disposable;
            if (m_ChildrenDic.TryGetValue(state, out disposable))
            {
                disposable.Dispose();

                m_ChildrenDic.Remove(state);
            }
        }

        private bool CheckContact(MultiCollider oppositeRoot)
        {
            switch (m_DetectionType)
            {
                case EDetectionType.Any:

                    return Children.Any(child => child.Opposites.Any(opposite => opposite.ContainsRoot(oppositeRoot)));

                case EDetectionType.All:

                    return Children.All(child => child.Opposites.Any(opposite => opposite.ContainsRoot(oppositeRoot)));
            }

            return false;
        }

        public void CheckEnter(MultiCollider oppositeRoot)
        {
            if (CheckContact(oppositeRoot))
            {
                m_EachEnter.OnNext(oppositeRoot);

                if (!m_OppositesList.Contains(oppositeRoot))
                {
                    m_OppositesList.Add(oppositeRoot);
                    m_Enter.OnNext(oppositeRoot);
                }
            }
        }

        public void CheckExit(MultiCollider oppositeRoot)
        {
            if (!CheckContact(oppositeRoot))
            {
                m_EachExit.OnNext(oppositeRoot);

                if (m_OppositesList.Contains(oppositeRoot))
                {
                    m_OppositesList.Remove(oppositeRoot);
                    m_Exit.OnNext(oppositeRoot);
                }
            }
        }

        #region IMultiCollider

        private Subject<IDetectable> m_Enter = new Subject<IDetectable>();

        public IObservable<IDetectable> OnEnter() => m_Enter;

        private Subject<IDetectable> m_Exit = new Subject<IDetectable>();

        public IObservable<IDetectable> OnExit() => m_Exit;

        private Subject<IDetectable> m_EachEnter = new Subject<IDetectable>();

        public IObservable<IDetectable> OnEachEnter() => m_EachEnter;

        private Subject<IDetectable> m_EachExit = new Subject<IDetectable>();

        public IObservable<IDetectable> OnEachExit() => m_EachExit;

        public IDisposable Subscribe(IDetector detector)
        {
            CompositeDisposable disposables = new CompositeDisposable();

            OnEnter().Subscribe(detector.OnEnter).AddTo(detector.gameObject).AddTo(this).AddTo(disposables);
            OnExit().Subscribe(detector.OnExit).AddTo(detector.gameObject).AddTo(this).AddTo(disposables);

            OnEachEnter().Subscribe(detector.OnEachEnter).AddTo(detector.gameObject).AddTo(this).AddTo(disposables);
            OnEachExit().Subscribe(detector.OnEachExit).AddTo(detector.gameObject).AddTo(this).AddTo(disposables);

            return disposables;
        }

        #endregion IMultiCollider
    }
}