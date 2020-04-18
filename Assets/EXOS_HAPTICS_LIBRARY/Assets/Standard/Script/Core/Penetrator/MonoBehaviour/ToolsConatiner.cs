using exiii.Extensions;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public class ToolsConatiner : PenetratorContainerBase
    {
        [SerializeField, Unchangeable, FormerlySerializedAs("ToolsHolders")]
        private ToolsHolder[] m_ToolsHolders;

        [SerializeField, FormerlySerializedAs("CollectionType")]
        private CollectionType m_CollectionType = CollectionType.SumAndMaxClamp;

        [SerializeField, FormerlySerializedAs("ApplyFilter")]
        private bool m_ApplyFilter = true;

        private float m_DistanceLimitGain = 0.01f;
        private float m_RotateLimitGain = 0.001f;

        private OrientedSegment m_TargetSegment;
        private OrientedSegment m_ClosestSegment;

        public bool IsCollided
        {
            get { return m_ToolsHolders.Any(x => x.IsCollided); }
        }

        public OrientedSegment ClosestSegment
        {
            get { return m_ClosestSegment; }
        }

        private IReadOnlyCollection<IPenetrator> m_Penetrators;
        public override IReadOnlyCollection<IPenetrator> Penetrators
        {
            get { return m_Penetrators; }
        }

        protected override void Awake()
        {
            base.Awake();

            m_ClosestSegment = OrientedSegment.zero;
            m_TargetSegment = OrientedSegment.zero;
        }

        protected override void Start()
        {
            base.Start();

            // collect current holders.
            CollectHolders();

            this.UpdateAsObservable()
                .Subscribe(_ => LineDrawerGL.DrawLine(ClosestSegment, Color.magenta));

            this.FixedUpdateAsObservable()
                .Subscribe(_ => UpdateSegment());
        }

        // collect current holders.
        public void CollectHolders()
        {
            var holders = GetComponentsInChildren<ToolsHolder>();            

            m_ToolsHolders = holders;

            m_Penetrators = holders.Select(h => h.Penetrator).ToArray();
        }

        private void UpdateSegment()
        {
            if (m_ToolsHolders.Count() == 0) { return; }

            var segmentInActive = m_ToolsHolders.Where(x => x.IsCollided);

            if (segmentInActive.Count() == 0 || segmentInActive.Where(holder => holder.ClosestSegment.HasLength).Count() == 0)
            {
                m_TargetSegment.InitialPoint = ExtensionVector.Average(m_ToolsHolders.Select(holder => holder.InitialPoint.position));
                m_TargetSegment.TerminalPoint = m_TargetSegment.InitialPoint;
            }
            else
            {
                m_TargetSegment.InitialPoint = OrientedSegment.WeightedAverageOfInitial(segmentInActive.Select(x => x.ClosestSegment));

                switch (m_CollectionType)
                {
                    case CollectionType.SumAndMax:
                        m_TargetSegment.TerminalPoint = m_TargetSegment.InitialPoint + ExtensionVector.Sum(segmentInActive.Select(x => x.ClosestSegment.Vector));
                        m_TargetSegment.Length = m_ToolsHolders.Max(x => x.ClosestSegment.Length);
                        break;

                    case CollectionType.Average:
                        m_TargetSegment.TerminalPoint = m_TargetSegment.InitialPoint + ExtensionVector.Average(m_ToolsHolders.Select(x => x.ClosestSegment.Vector));
                        break;

                    case CollectionType.AverageInActive:
                        m_TargetSegment.TerminalPoint = m_TargetSegment.InitialPoint + ExtensionVector.Average(segmentInActive.Select(x => x.ClosestSegment.Vector));
                        break;

                    case CollectionType.SumAndMaxClamp:
                        m_TargetSegment.TerminalPoint = m_TargetSegment.InitialPoint + ExtensionVector.Sum(segmentInActive.Select(x => x.ClosestSegment.Vector));
                        var max = m_ToolsHolders.Max(x => x.ClosestSegment.Length);
                        if (m_TargetSegment.Length > max) { m_TargetSegment.Length = max; }
                        break;
                }
            }

            if (m_ApplyFilter)
            {
                m_ClosestSegment = m_ClosestSegment.RotateTowardsAsVector(m_TargetSegment, Time.fixedTime * m_RotateLimitGain, Time.fixedTime * m_DistanceLimitGain);
            }
            else
            {
                m_ClosestSegment = m_TargetSegment;
            }
        }
    }
}
