using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using exiii.Extensions;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;

using Grippable = exiii.Unity.IManipulable<exiii.Unity.IGripManipulation>;

namespace exiii.Unity
{
	public abstract class GripBehaviourTwoFinger : GripBehaviourBase
    {
		#region Inspector

		[Header(nameof(GripBehaviourTwoFinger))]

        [SerializeField]
        private Transform m_ReferenceTransformA;

        [SerializeField]
        private Transform m_ReferenceTransformB;

        [SerializeField]
        private GrippableDetector m_GrippableDetectorA;

        [SerializeField]
        private GrippableDetector m_GrippableDetectorB;

        [SerializeField]
        private bool m_SelfRotation = false;

        #endregion Inspector

        private IEnumerable<Grippable> m_Intersect;

        public abstract bool AllowGrip { get; }

        protected virtual void Awake()
        {
            if (m_GrippableDetectorA == null)
            {
                m_GrippableDetectorA = m_ReferenceTransformA.GetComponentInChildren<GrippableDetector>();
            }

            if (m_GrippableDetectorB == null)
            {
                m_GrippableDetectorB = m_ReferenceTransformB.GetComponentInChildren<GrippableDetector>();
            }

            m_Intersect = m_GrippableDetectorA.Targets.Intersect(m_GrippableDetectorB.Targets);

            this.UpdateAsObservable().Subscribe(_ => UpdateGrip()).AddTo(this);
        }

        private void UpdateGrip()
        {
            UpdateGripPosition();

            CheckGrip();
        }

        private void UpdateGripPosition()
        {
            transform.position = m_ReferenceTransformA.position + (m_ReferenceTransformB.position - m_ReferenceTransformA.position) * PositionRatio;

            if (m_SelfRotation) { transform.LookAt(m_ReferenceTransformA); }

            Distance = (m_ReferenceTransformB.position - m_ReferenceTransformA.position).magnitude;
        }

        private void CheckGrip()
        {
            if (AllowGrip)
            {
                // HACK: Need optimize
                GrippedItems.ToArray().Foreach(CheckGrippedItem);

                m_Intersect.ToArray().Foreach(CheckIntersectedItem);
            }
            else
            {
                GrippedItems.ToArray().Foreach(EndGrip);
            }
        }

        private void CheckGrippedItem(Grippable grippable)
        {
            if (m_Intersect.Contains(grippable))
            {
                StayGrip(grippable);
            }
            else
            {
                EndGrip(grippable);
            }
        }

        private void CheckIntersectedItem(Grippable grippable)
        {
            if (!GrippedItems.Contains(grippable))
            {
                StartGrip(grippable);
            }
        }
    }
}
