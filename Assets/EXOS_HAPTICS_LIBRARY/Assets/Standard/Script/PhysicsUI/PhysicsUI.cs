using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace exiii.Unity.PhysicsUI
{
    [Flags]
    public enum EPhysicUIButtonState
    {
        None = 1 << 0,
        Down = 1 << 1,
        Stay = 1 << 2,
        Up = 1 << 3,
    }
}
