using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;
using exiii.Unity.Rx;

namespace exiii.Unity.PhysicsUI
{
    public class PhysicsUISnap : InteractableNode, ISurfaceForceGenerator
    {
        [SerializeField]
        [FormerlySerializedAs("Duration")]
        private float m_Duration = 0.1f;

        [SerializeField]
        [FormerlySerializedAs("SurfaceGain")]
        private float m_SurfaceGain = 1.0f;

        public bool IsSnap { get; private set; } = false;

        // start snap.
        public void Snap()
        {
            IsSnap = true;

            Observable.Timer(TimeSpan.FromSeconds(m_Duration))
                .Subscribe(_ => { IsSnap = false; })
                .AddTo(this);
        }

        public void OnGenerate(IForceReceiver receiver, ISurfaceState state)
        {
            if (!IsSnap) { return; }

            Vector3 NormalVector = this.transform.up;
            receiver.AddForceRatio(state.PointOnSurface, m_SurfaceGain * NormalVector);
        }
    }
}
