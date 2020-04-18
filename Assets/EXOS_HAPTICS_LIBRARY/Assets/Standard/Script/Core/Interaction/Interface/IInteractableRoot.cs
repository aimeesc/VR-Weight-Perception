using UnityEngine;

namespace exiii.Unity
{
    public interface IInteractableRoot : IRootScript,
        IManipulable,
        ILinkTo<PhysicalProperties>
    {
        bool IsPhysicalObject { get; }
        bool IsStickyGrab { get; }

        PhysicalProperties PhysicalProperties { get; }
        Rigidbody Rigidbody { get; }
    }
}