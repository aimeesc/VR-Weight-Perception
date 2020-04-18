using exiii.Extensions;
using exiii.Unity.Rx;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public abstract class ColliderManipulator<TInterface, TClass> : CycleManipulator<TInterface, TClass>, IDetector, IDetector<IManipulable<TInterface>>
        where TInterface : class, ICycleManipulation<TInterface>
        where TClass : ColliderManipulation<TInterface>, TInterface
    {
        #region Inspector

        [Header("ColliderManipulator")]
        [SerializeField]
        [FormerlySerializedAs("m_RootMultiCollider")]
        private MultiCollider m_MultiCollider;

        [SerializeField]
        private bool m_ManipulateOnEachEnter = false;

        #endregion Inspector

        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_MultiCollider == null)
            {
                m_MultiCollider = transform.GetOrAddComponent<MultiCollider>();
            }
        }

        /*
        protected override void Start()
        {
            base.Start();

            if (m_MultiCollider == null) { return; }

            m_MultiCollider.Subscribe(this).AddTo(Disposer);
        }
        */

        public override void Initialize()
        {
            base.Initialize();

            if (m_MultiCollider == null) { return; }

            m_MultiCollider.Subscribe(this).AddTo(Disposer);
        }

        public void OnEnter(IDetectable multiCollider)
        {
            if (multiCollider.RootGameObject == null) { return; }

            var targets = multiCollider.RootGameObject.GetComponents<IManipulable<TInterface>>();

            targets.Foreach(x => OnEnter(x));
        }

        public void OnExit(IDetectable multiCollider)
        {
            if (multiCollider.RootGameObject == null) { return; }

            var targets = multiCollider.RootGameObject.GetComponents<IManipulable<TInterface>>();

            targets.Foreach(x => OnExit(x));
        }

        public void OnEachEnter(IDetectable multiCollider)
        {
            if (m_ManipulateOnEachEnter == false) { return; }

            if (multiCollider.RootGameObject == null) { return; }

            var targets = multiCollider.RootGameObject.GetComponents<IManipulable<TInterface>>();

            targets.Foreach(x => OnEachEnter(x));
        }

        public void OnEachExit(IDetectable multiCollider)
        {
            if (m_ManipulateOnEachEnter == false) { return; }

            if (multiCollider.RootGameObject == null) { return; }

            var targets = multiCollider.RootGameObject.GetComponents<IManipulable<TInterface>>();

            targets.Foreach(x => OnEachExit(x));
        }

        public abstract void OnEnter(IManipulable<TInterface> component);

        public abstract void OnExit(IManipulable<TInterface> component);

        public virtual void OnEachEnter(IManipulable<TInterface> component)
        {
        }

        public virtual void OnEachExit(IManipulable<TInterface> component)
        {
        }
    }
}