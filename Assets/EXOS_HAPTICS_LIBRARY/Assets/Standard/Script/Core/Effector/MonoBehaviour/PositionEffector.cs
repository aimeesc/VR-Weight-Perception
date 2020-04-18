using System;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity
{
    public class PositionEffector : InteractorNode, IPositionEffector
    {
        [Header(nameof(PositionEffector))]
        [SerializeField, Unchangeable]
        private Vector3 m_PossitionEffect;

        [SerializeField, Unchangeable]
        private Quaternion m_RotationEffect;

        private Vector3 m_InitialPosition;
        private Quaternion m_InitialRotation;

        protected override void Awake()
        {
            base.Awake();

            m_InitialPosition = transform.localPosition;
            m_InitialRotation = transform.localRotation;
        }

        protected override void Start()
        {
            base.Start();

            IObservable<IPositionState> observable;
            if (InteractorRoot != null && InteractorRoot.TryGetStateObservable(out observable))
            {
                observable.Subscribe(OnUpdatePosition);
            }
        }

        #region IPositionEffector

        public void OnUpdatePosition(IPositionState state)
        {
            m_PossitionEffect = state.Position;
            m_RotationEffect = state.Rotation;

            // HACK: need correspond to rotation
            transform.localPosition = m_InitialPosition + transform.InverseTransformVector(m_PossitionEffect);
        }

        public void OnResetPosition()
        {
            transform.localPosition = m_InitialPosition;
            transform.localRotation = m_InitialRotation;
        }

        #endregion IPositionEffector
    }
}