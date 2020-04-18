using System;
using UnityEngine;
using UnityEngine.Serialization;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;

namespace exiii.Unity
{
    public class StateReplicator : ExMonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("TargetTransform")]
        protected Transform m_TargetTransform = null;

        protected ReactiveProperty<bool> m_Active = null;

        // on awake.
        protected override void Awake()
        {
            base.Awake();

            m_Active = new ReactiveProperty<bool>();
            m_Active.Subscribe(_ => OnChangeActive(_)).AddTo(this);

            // update target state;
            this.FixedUpdateAsObservable()
                .Subscribe(_ => UpdateTargetState())
                .AddTo(this);
        }

        // is active.
        public bool IsActiveObject()
        {
            if (m_TargetTransform == null) { return false; }

            return true;
        }

        // init target.
        public void InitTarget(Transform target)
        {
            m_TargetTransform = target;
        }

        // update target state.
        public void UpdateTargetState()
        {
            if (!IsActiveObject()) { return; }

            // replicate active state.
            m_Active.Value = m_TargetTransform.gameObject.activeInHierarchy;
        }

        // on change active.
        protected virtual void OnChangeActive(bool active)
        {
        }
    }
}
