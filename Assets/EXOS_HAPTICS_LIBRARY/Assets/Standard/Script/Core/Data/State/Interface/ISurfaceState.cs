using UnityEngine;

namespace exiii.Unity
{
    public interface ISurfaceState : ITouchState
    {
        Vector3 PointOnSurface { get; }

        float Penetration { get; }

        Material TargetMaterial { get; }

        int SubMeshIndex { get; }

        Vector3 BumpVector { get; }

        Vector3 NormalVector { get; }
    }
}