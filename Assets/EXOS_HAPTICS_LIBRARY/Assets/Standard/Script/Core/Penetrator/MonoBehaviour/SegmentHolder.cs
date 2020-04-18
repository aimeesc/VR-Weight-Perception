using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using UnityEngine;

namespace exiii.Unity
{
    public abstract class SegmentHolder : PenetratorHolder
    {
        #region Inspector

        [Header(nameof(SegmentHolder))]
        [SerializeField]
        private Transform m_InitialPoint;

        public Transform InitialPoint
        {
            get { return m_InitialPoint; }
            set { m_InitialPoint = value; }
        }

        [SerializeField]
        private Transform m_TerminalPoint;

        public Transform TerminalPoint
        {
            get { return m_TerminalPoint; }
            set { m_TerminalPoint = value; }
        }

        [SerializeField, Unchangeable]
        private OrientedSegment m_ClosestSegment = OrientedSegment.zero;

        public OrientedSegment ClosestSegment
        {
            get
            {
                m_ClosestSegment.InitialPoint = m_InitialPoint.position;
                m_ClosestSegment.TerminalPoint = m_TerminalPoint.position;

                return m_ClosestSegment;
            }
        }

        #endregion Inspector
    }
}