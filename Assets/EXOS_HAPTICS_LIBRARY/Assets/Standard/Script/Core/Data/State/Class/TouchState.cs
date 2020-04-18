using System;
using UnityEngine;

namespace exiii.Unity
{
    public class TouchState : ITouchState
    {
        public ITouchManipulator Manipulator { get; set; }

        public TouchForceParameter TouchForceParameter => Manipulator.TouchForceParameter;

        public TouchState(ITouchManipulator manipulator)
        {
            Manipulator = manipulator;
        }

        public TouchState(ITouchState state)
        {
            Manipulator = state.Manipulator;
        }
    }
}