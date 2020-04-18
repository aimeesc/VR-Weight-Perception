using exiii.Extensions;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

using Grippable = exiii.Unity.IManipulable<exiii.Unity.IGripManipulation>;

namespace exiii.Unity
{
    /// <summary>
    /// Gripper's waypoint between Thamb and Index
    /// </summary>
    public abstract class GripBehaviourBase : MonoBehaviour, IGripBehaviour
    {
        #region Inspector

        [Header(nameof(GripBehaviourBase))]
        [SerializeField]
        [FormerlySerializedAs("PositionRatio")]
        private float m_PositionRatio = 0.5f;

        public float PositionRatio { get { return m_PositionRatio; } }

        [SerializeField, Unchangeable]
        private float m_Distance = 0.5f;

        public float Distance
        {
            get { return m_Distance; }
            set { m_Distance = value; }
        }

        #endregion Inspector

        private HashSet<Grippable> m_GrippedItems = new HashSet<Grippable>();

        public IReadOnlyCollection<Grippable> GrippedItems { get { return m_GrippedItems; } }

        /// <summary>
        /// Class of Grip event
        /// </summary>
        protected ExEventSet<Grippable> m_GripEvent = new ExEventSet<Grippable>();

        /// <summary>
        /// Interface of Grip event
        /// </summary>
        public IExEventSet<Grippable> GripEvent { get { return m_GripEvent; } }

        public virtual Transform Center { get { return transform; } }

        protected void StartGrip(Grippable grippable)
        {
            m_GrippedItems.Add(grippable);
            m_GripEvent.Start.OnNext(grippable);
        }

        protected void StayGrip(Grippable grippable)
        {
            m_GripEvent.Stay.OnNext(grippable);
        }

        protected void EndGrip(Grippable grippable)
        {
            m_GripEvent.End.OnNext(grippable);
            m_GrippedItems.Remove(grippable);
        }
    }
}