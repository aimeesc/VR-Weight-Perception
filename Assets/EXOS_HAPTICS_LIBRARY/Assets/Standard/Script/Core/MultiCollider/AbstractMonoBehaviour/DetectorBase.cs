using exiii.Extensions;
using UnityEngine;

namespace exiii.Unity
{
    public abstract class DetectorBase : ExMonoBehaviour, IDetector
    {
        [Header(nameof(DetectorBase))]
        [SerializeField]
        private MultiCollider m_MultiCollider;

        public MultiCollider MultiCollider
        {
            get { return m_MultiCollider; }
            set { m_MultiCollider = value; }
        }

        protected override void Start()
        {
            base.Start();

            if (m_MultiCollider != null)
            {
                MultiCollider.Subscribe(this);
            }
            else
            {
                EHLDebug.LogWarning($"{nameof(DetectorBase)}.{nameof(Start)} : m_RootMultiCollider is null", this);
            }
        }

        public abstract void OnEnter(IDetectable contact);

        public abstract void OnExit(IDetectable contact);

        public virtual void OnEachEnter(IDetectable contact)
        {
        }

        public virtual void OnEachExit(IDetectable contact)
        {
        }
    }

    public abstract class DetectorBase<TType> : DetectorBase, IDetector<TType>
    {
        public override void OnEnter(IDetectable multiCollider)
        {
            if (multiCollider.RootGameObject == null) { return; }

            var targets = multiCollider.RootGameObject.GetComponents<TType>();

            targets.Foreach(x => OnEnter(x));
        }

        public override void OnExit(IDetectable multiCollider)
        {
            if (multiCollider.RootGameObject == null) { return; }

            var targets = multiCollider.RootGameObject.GetComponents<TType>();

            targets.Foreach(x => OnExit(x));
        }

        public abstract void OnEnter(TType contact);

        public abstract void OnExit(TType contact);

        public virtual void OnEachEnter(TType contact)
        {
        }

        public virtual void OnEachExit(TType contact)
        {
        }
    }
}