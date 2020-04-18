using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public class ModelController : InteractorNode, IPositionEffector
    {
        #region Inspector

        [Header(nameof(ModelController))]
        [SerializeField]
        [FormerlySerializedAs("m_TrackingParameter")]
        private TrackingSettings m_TrackingSettings;

        [SerializeField]
        private Transform m_TargetTransform;

        [SerializeField]
        private bool m_AutoDetectTargetTransform = true;

        [SerializeField]
        private Vector3 m_PositionOffset;

        [SerializeField]
        private Vector3 m_RotationOffset;

        #endregion Inspector

        protected override void Start()
        {
            base.Start();

            if (InteractorRoot == null)
            {
                enabled = false;
                return;
            }

            m_TrackingSettings = InteractorRoot.TrackingSettings;

            if (m_AutoDetectTargetTransform)
            {
                var origins = GetComponentsInChildren<ITrackingOrigin>();

                var origin = origins
                    .Where(x => m_TrackingSettings.Equals(x.TrackingParameter))
                    .FirstOrDefault();

                if (origin != null)
                {
                    m_TargetTransform = origin.transform;
                }
                else
                {
                    Debug.LogWarning($"[{InteractorRoot.ExName}] PositionOffset : ExosOrigin [{m_TrackingSettings.TrackingType} + {m_TrackingSettings.TrackingPosition}] is not found.");
                    enabled = false;
                }
            }

            IObservable<IPositionState> observable;
            if (InteractorRoot != null && InteractorRoot.TryGetStateObservable(out observable))
            {
                observable.Subscribe(OnUpdatePosition);
            }
        }

        #region IPositionEffector

        public void OnUpdatePosition(IPositionState state)
        {
            if (m_TargetTransform == null) { return; }

            transform.localRotation = Quaternion.Euler(m_RotationOffset) * Quaternion.Inverse(m_TargetTransform.rotation) * transform.rotation;
            transform.localPosition = m_PositionOffset + transform.localRotation * transform.InverseTransformVector(transform.position - m_TargetTransform.position);
        }

        public void OnResetPosition()
        {

        }

        #endregion IPositionEffector
    }
}