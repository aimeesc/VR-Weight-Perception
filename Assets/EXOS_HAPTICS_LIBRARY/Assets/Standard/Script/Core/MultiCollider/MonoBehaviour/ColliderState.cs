using exiii.Extensions;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity
{
    public class ColliderState : ExMonoBehaviour
    {
        #region Inspector

        [Header(nameof(ColliderState))]
        [SerializeField]
        private Collider m_Collider;

        public Collider Collider
        {
            get { return m_Collider; }
            set { m_Collider = value; }
        }

        [Header("DebugInspector")]
        [SerializeField]
        private MultiCollider[] m_MultiColliders;

        [SerializeField]
        private ColliderState[] m_Opposites;

        //[SerializeField]
        private bool m_UseInternalCheck = false;

        #endregion Inspector

        private HashSet<ColliderState> m_WasEnter = new HashSet<ColliderState>();
        private HashSet<ColliderState> m_WasExit = new HashSet<ColliderState>();

        private ReactiveDictionary<MultiCollider, IDisposable> m_RootDic = new ReactiveDictionary<MultiCollider, IDisposable>();

        public ICollection<MultiCollider> Roots => m_RootDic.Keys;

        private ReactiveDictionary<ColliderState, IDisposable> m_OppositesDic = new ReactiveDictionary<ColliderState, IDisposable>();

        public ICollection<ColliderState> Opposites => m_OppositesDic.Keys;

        //public ICollection<ColliderState> OppositeColliders => m_OppositesDic.Keys.Where(x => !x.Collider.isTrigger).ToArray();

        //public ICollection<ColliderState> OppositeTriggers => m_OppositesDic.Keys.Where(x => x.Collider.isTrigger).ToArray();

        protected override void Start()
        {
            base.Start();

            this.FixedUpdateAsObservable().Subscribe(_ => UpdateRootState());

            if (EHLDebug.DebugInspector)
            {
                SetupDebugInspector();
            }
        }

        private void SetupDebugInspector()
        {
            m_MultiColliders = m_RootDic.Keys.ToArray();
            m_Opposites = m_OppositesDic.Keys.ToArray();

            m_RootDic
                .ObserveCountChanged()
                .Subscribe(_ => m_MultiColliders = m_RootDic.Keys.ToArray());

            m_OppositesDic
                .ObserveCountChanged()
                .Subscribe(_ => m_Opposites = m_OppositesDic.Keys.ToArray());
        }

        public override void Terminate()
        {
            base.Terminate();

            RemoveOppositeAll();
        }

        private void UpdateRootState()
        {
            if (m_WasEnter.Count > 0) { CheckRootEnter(); }
            if (m_WasExit.Count > 0) { CheckRootExit(); }
        }

        public IObservable<bool> OnColliderIsTriggerChanged()
        {
            return m_Collider.ObserveEveryValueChanged(x => x.isTrigger);
        }

        public IObservable<bool> OnColliderIsTriggerChangedTo(bool state)
        {
            return OnColliderIsTriggerChanged().Where(value => value == state);
        }

        public IObservable<bool> OnColliderEnableChanged()
        {
            return m_Collider.ObserveEveryValueChanged(x => x.enabled);
        }

        public IObservable<bool> OnColliderEnableChangedTo(bool state)
        {
            return OnColliderEnableChanged().Where(value => value == state);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!Collider.enabled) { return; }

            var opposites = collider.GetComponents<ColliderState>();

            if (opposites == null) { return; }

            var opposite = opposites.Where(x => x.Collider == collider).FirstOrDefault();

            if (opposite == null) { return; }

            AddOpposite(opposite);
        }

        public void AddOpposite(ColliderState opposite)
        {
            if (opposite == null) { return; }

            if (m_OppositesDic.ContainsKey(opposite)) { return; }

            var disposables = new CompositeDisposable();

            disposables.AddTo(this);

            this.OnColliderEnableChangedTo(false)
                .Subscribe(_ => opposite.RemoveOpposite(this))
                .AddTo(this)
                .AddTo(disposables);

            this.OnDisableAsObservable()
                .Subscribe(_ => opposite.RemoveOpposite(this))
                .AddTo(this)
                .AddTo(disposables);

            m_OppositesDic.Add(opposite, disposables);

            m_WasEnter.Add(opposite);

            //Roots.Foreach(root => opposite.Roots.Foreach(oppositeRoot => root.CheckEnter(oppositeRoot)));
        }

        private void CheckRootEnter()
        {
            Roots.Foreach(root => m_WasEnter.Foreach(opposite => opposite.Roots.Foreach(oppositeRoot => root.CheckEnter(oppositeRoot))));

            m_WasEnter.Clear();
        }

        private void OnTriggerExit(Collider collider)
        {
            if (!Collider.enabled) { return; }

            var opposites = collider.GetComponents<ColliderState>();

            if (opposites == null) { return; }

            var opposite = opposites.Where(x => x.Collider == collider).FirstOrDefault();

            if (opposite == null) { return; }

            var mesh = collider as MeshCollider;

            if (m_UseInternalCheck && mesh != null && !mesh.convex )
            {
                if (!ExPhysics.CheckInternalForClosedMesh(mesh, transform.position))
                {
                    RemoveOpposite(opposite);
                }
            }
            else
            {
                RemoveOpposite(opposite);
            }
        }

        public bool RemoveOpposite(ColliderState opposite)
        {
            if (opposite == null) { return false; }

            IDisposable disposable;
            if (!m_OppositesDic.TryGetValue(opposite, out disposable)) { return false; }

            disposable.Dispose();

            m_OppositesDic.Remove(opposite);

            m_WasExit.Add(opposite);

            //Roots.Foreach(root => opposite.Roots.Foreach(oppositeRoot => root.CheckExit(oppositeRoot)));

            return true;
        }

        public void RemoveOppositeAll()
        {
            while (m_OppositesDic.Keys.Count() > 0)
            {
                if (RemoveOpposite(m_OppositesDic.Keys.First()) == false) { break; }
            }
        }

        private void CheckRootExit()
        {
            Roots.Foreach(root => m_WasExit.Foreach(opposite => opposite.Roots.Foreach(oppositeRoot => root.CheckExit(oppositeRoot))));

            m_WasExit.Clear();
        }

        public bool AddRootUnique(MultiCollider multiCollider)
        {
            if (Roots.Contains(multiCollider)) { return false; }

            var disposables = new CompositeDisposable();

            disposables.AddTo(this);

            this.OnColliderEnableChangedTo(false)
                .Subscribe(_ => multiCollider.RemoveChildren(this))
                .AddTo(this)
                .AddTo(disposables);

            this.OnDisableAsObservable()
                .Subscribe(_ => multiCollider.RemoveChildren(this))
                .AddTo(this)
                .AddTo(disposables);

            m_RootDic.Add(multiCollider, disposables);
            return true;
        }

        public bool ContainsRoot(MultiCollider root)
        {
            return Roots.Contains(root);
        }

        public bool RemoveRoot(MultiCollider root)
        {
            IDisposable disposable;
            if (!m_RootDic.TryGetValue(root, out disposable)) { return false; }

            disposable.Dispose();

            m_RootDic.Remove(root);
            return true;
        }

        public IObservable<int> OnOppositesCountChanged()
        {
            return m_OppositesDic.ObserveCountChanged();
        }

        public bool RootsHasTag(IExTag tag, EDetectionType forRoots = EDetectionType.Any)
        {
            switch (forRoots)
            {
                case EDetectionType.Any:
                    return Roots.Any(root => tag.CheckTag(root.ExTags));

                case EDetectionType.All:
                    return Roots.All(root => tag.CheckTag(root.ExTags));

                default:
                    throw new ArgumentOutOfRangeException($"{nameof(RootsHasTag)} : {nameof(EDetectionType)}");
            }
        }

        public bool RootsHasTag(IEnumerable<IExTag> tags, EDetectionType forTags = EDetectionType.Any, EDetectionType forRoots = EDetectionType.Any)
        {
            switch (forTags)
            {
                case EDetectionType.Any:
                    return tags.Any(tag => RootsHasTag(tag, forRoots));

                case EDetectionType.All:
                    return tags.All(tag => RootsHasTag(tag, forRoots));

                default:
                    throw new ArgumentOutOfRangeException($"{nameof(RootsHasTag)} : {nameof(EDetectionType)}");
            }
        }
    }
}