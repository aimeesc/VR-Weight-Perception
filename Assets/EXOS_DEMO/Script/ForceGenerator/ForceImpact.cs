using exiii.Unity.EXOS;
using System;
using exiii.Unity.Rx;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class ForceImpact : MonoBehaviour, IGrabForceGenerator
    {
        [SerializeField]
        protected Transform m_ForceOriginTransform;

        [SerializeField]
        protected float m_ForceGain = 1.0f;

        [SerializeField]
        protected float m_DeltaTime = 0.1f;

        [Header("Debug")]
        [SerializeField]
        protected Vector3 m_LocalForceOrigin;

        protected Vector3 ForceOrigin
        {
            get { return transform.TransformPoint(m_LocalForceOrigin); }
            set { m_LocalForceOrigin = transform.InverseTransformPoint(value); }
        }

        [SerializeField]
        protected Vector3 m_LocalDirection;

        protected Vector3 Direction
        {
            get { return transform.TransformDirection(m_LocalDirection); }
            set { m_LocalDirection = transform.InverseTransformDirection(value); }
        }

        public bool IsActive { get; private set; } = false;

        public void Impact()
        {
            IsActive = true;

            Observable.Timer(TimeSpan.FromSeconds(m_DeltaTime))
                .Subscribe(_ => { IsActive = false; })
                .AddTo(this);
        }

        public void OnGenerate(IForceReceiver receiver, IGrabState state)
        {
            if (!IsActive) { return; }

            if (m_ForceOriginTransform != null) { m_LocalForceOrigin = m_ForceOriginTransform.position; }

            receiver.AddForceRatio(ForceOrigin, Direction * m_ForceGain);

            EHLDebug.DrawLine(ForceOrigin, ForceOrigin + Direction * m_ForceGain, Color.cyan);
        }
    }
}