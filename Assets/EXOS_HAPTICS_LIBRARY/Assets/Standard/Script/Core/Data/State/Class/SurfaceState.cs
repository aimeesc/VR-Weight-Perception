using UnityEngine;

namespace exiii.Unity
{
    public class SurfaceState : TouchState, ISurfaceState
    {
        public Vector3 PointOnSurface { get; set; }

        public float Penetration { get; set; }

        public Material TargetMaterial { get; set; }

        public int SubMeshIndex { get; set; }

        public Vector3 BumpVector { get; set; }

        public Vector3 NormalVector { get; set; }

        public SurfaceState(ITouchManipulator manipulator) : base(manipulator) { }

        public SurfaceState(ITouchState state) : base(state) { }
    }
}