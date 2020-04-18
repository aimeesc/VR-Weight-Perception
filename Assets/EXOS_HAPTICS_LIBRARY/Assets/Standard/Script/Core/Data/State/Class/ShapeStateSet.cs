using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace exiii.Unity
{
    public class ShapeStateSet : TouchState, IShapeStateSet
    {
        private OrientedSegment m_SummarizedOutput;

        public OrientedSegment SummarizedOutput
        {
            get { return m_SummarizedOutput; }
        }

        private List<IShapeState> m_Collection = new List<IShapeState>();

        public IReadOnlyCollection<IShapeState> Collection { get { return m_Collection; } }

        public bool Enabled { get { return m_Collection.Count > 0; } }

        public bool ApplyFilter { get; set; } = true;

        public float DistanceLimitGain { get; set; } = 0.01f;
        public float RotateLimitGain { get; set; } = 0.001f;

        public ShapeStateSet(ITouchManipulator manipulator) : base(manipulator)
        {
        }

        public void UpdateSummarizedOutput(OrientedSegment target)
        {
            if (ApplyFilter)
            {
                m_SummarizedOutput = m_SummarizedOutput.RotateTowardsAsVector(target, Time.fixedTime * RotateLimitGain, Time.fixedTime * DistanceLimitGain);
            }
            else
            {
                m_SummarizedOutput = target;
            }
        }

        public void Add(IShapeState state)
        {
            m_Collection.Add(state);
        }

        public void Clear()
        {
            m_Collection.Clear();
        }
    }
}
