using exiii.Unity.Rx;
using System;
using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    /// Follow the target object
    /// </summary>
    public class FollowTransform : MonoBehaviour
    {
        #region Inspector

        [Header("TransformFollow")]
        [SerializeField]
        private Transform target;

        public Transform Target { get { return target; } }

        [SerializeField, Unchangeable]
        private Vector3 m_OffsetPosition;

        [SerializeField, Unchangeable]
        private Quaternion m_OffsetRotation;

        [SerializeField]
        private float m_LerpRatio = 1.0f;

        #endregion Inspector

        bool m_Enable = true;
        IDisposable m_ResumeTimer;

        public Vector3 Position
        {
            get { return transform.position; }
        }

        public Quaternion Rotation
        {
            get { return transform.rotation; }
        }

        public Vector3 LocalScale
        {
            get { return transform.localScale; }
        }

        private void OnValidate()
        {
            if (!this.IsPrefab())
            {
                UpdateOffset();
            }
        }

        private void OnEnable()
        {
            UpdateTransform();
        }

        public void Update()
        {
            UpdateTransform();
        }

        private void UpdateTransform()
        {
            if (target == null || m_Enable == false) { return; }

            transform.rotation = target.rotation * m_OffsetRotation;
            transform.position = Vector3.Lerp(transform.position, target.position + target.rotation * m_OffsetPosition, m_LerpRatio);
        }

        public void Follow(Transform newTarget)
        {
            if (newTarget == null) { return; }

            target = newTarget;

            UpdateOffset();
        }

        private void UpdateOffset()
        {
            if (target == null) { return; }

            var invRot = Quaternion.Inverse(target.rotation);

            m_OffsetPosition = invRot * (transform.position - target.position);
            m_OffsetRotation = invRot * transform.rotation;
        }

        public void Suspend()
        {
            if (m_ResumeTimer != null)
            {
                m_ResumeTimer.Dispose();
                m_ResumeTimer = null;
            }

            m_Enable = false;
        }

        public void Resume()
        {
            if (m_ResumeTimer != null)
            {
                m_ResumeTimer = null;
            }

            m_Enable = true;
        }

        public void ResumeWithDeray(float sec)
        {
            m_ResumeTimer = Observable
                .Timer(TimeSpan.FromSeconds(sec))
                .First()
                .Subscribe(_ => Resume())
                .AddTo(this);
        }

        public void Stop()
        {
            if (m_ResumeTimer != null)
            {
                m_ResumeTimer.Dispose();
                m_ResumeTimer = null;
            }

            if (target == null) { return; }

            target = null;

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}