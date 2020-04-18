using UnityEngine;

namespace exiii.Unity
{
    public interface ITouchState : IState
    {
        ITouchManipulator Manipulator { get; }

        TouchForceParameter TouchForceParameter { get; }
    }
}