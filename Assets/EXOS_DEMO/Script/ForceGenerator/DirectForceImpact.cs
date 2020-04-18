using exiii.Unity.EXOS;
using System;
using exiii.Unity.Rx;
using UnityEngine;
using System.Collections;
using exiii.Unity.Device;

namespace exiii.Unity.Sample
{
    public class DirectForceImpact : MonoBehaviour, IExosGrabForceGenerator
    {
        [SerializeField]
        protected EAxisType m_AxisType;

        [SerializeField]
        protected float m_ForceRatio = 1.0f;

        [SerializeField]
        protected float m_DeltaTime = 0.1f;

        [SerializeField]
        protected float m_DelayTime = 0.0f;

        public bool IsActive { get; private set; } = false;
        public bool IsDelay { get; private set; } = false;

        public void Impact()
        {
            IsDelay = true;
            IsActive = true;

            Observable.Timer(TimeSpan.FromSeconds(m_DelayTime))
                .Subscribe(_ => { IsDelay = false; })
                .AddTo(this);

            Observable.Timer(TimeSpan.FromSeconds(m_DelayTime + m_DeltaTime))
                .Subscribe(_ => { IsActive = false; })
                .AddTo(this);
        }

        public void OnGenerate(IForceReceiver receiver, IGrabState state)
        {

        }

        public void OnGenerate(IExosForceReceiver receiver, IGrabState state)
        {
            if (!IsActive) { return; }
            if (IsDelay) { return; }
            
            receiver.AddDirectForceRatio(m_AxisType, m_ForceRatio);
        }
    }
}