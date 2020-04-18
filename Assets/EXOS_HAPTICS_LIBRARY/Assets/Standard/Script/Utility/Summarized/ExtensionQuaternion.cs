using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public static class ExtensionQuaternion
    {
        public static bool IsNaN(this Quaternion quaternion)
        {
            return (float.IsNaN(quaternion.x) || float.IsNaN(quaternion.y) || float.IsNaN(quaternion.z) || float.IsNaN(quaternion.w));
        }
    }
}