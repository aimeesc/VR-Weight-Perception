using UnityEngine;
using System.Collections;
using System;

namespace exiii.Unity
{
    public class ShapeState : TouchState, IShapeState
    {
        public OrientedSegment LocalClosestSegment { get; set; }

        public OrientedSegment ClosestSegment
        {
            get
            {
                return OrientedSegment.Transform(LocalClosestSegment, Manipulator.transform);
            }

            set
            {
                LocalClosestSegment = OrientedSegment.InverseTransform(value, Manipulator.transform);
            }
        }

        public ShapeState(ITouchManipulator manipulator) : base(manipulator) { }

        public ShapeState(ITouchManipulator manipulator, OrientedSegment segment) : base(manipulator)
        {
            ClosestSegment = segment;
        }

        public ShapeState(ITouchState state) : base(state) { }

        public ShapeState(ITouchState state, OrientedSegment segment) : base(state)
        {
            ClosestSegment = segment;
        }
    }
}
