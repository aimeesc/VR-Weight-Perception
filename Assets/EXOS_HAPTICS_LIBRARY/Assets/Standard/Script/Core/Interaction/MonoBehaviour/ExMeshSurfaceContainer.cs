using exiii.Extensions;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    ///
    /// </summary>
    [RequireComponent(typeof(MeshContainerBase))]
    public class ExMeshSurfaceContainer : SurfaceContainerBase
    {
        [SerializeField]
        private bool m_DoubleCheck = false;

        protected ExMesh[] m_ExMeshes;

        // HACK: Need correspond multiple meshes
        protected ExMesh m_ActiveMesh;

        public ExMesh ActiveMesh { get { return m_ActiveMesh; } }

        protected override void Start()
        {
            base.Start();

            var container = GetComponent<MeshContainerBase>();

            // HACK: Need change to implementation corresponding to array
            if (container == null || container.ExMeshes.ElementAt(0) == null) { gameObject.SetActive(false); }

            m_ExMeshes = container.ExMeshes.ToArray();

            m_ExMeshes.Foreach(x => x.MakeKDTree(m_DoubleCheck));

            m_ActiveMesh = m_ExMeshes[0];
        }

        protected override bool TryCalcPenetration(IPenetrator penetrator, out OrientedSegment penetration)
        {
            Vector3 closestPoint;
            var isInternal = m_ActiveMesh.CalcClosestPointOnSurface(penetrator.Center, out closestPoint);

            var result = new PenetrationStatus(penetrator.Center, closestPoint, isInternal);

            return penetrator.TryCalcCorrection(result, out penetration);
        }

        /*
        public bool TryCalcSurfaceResult(OrientedSegment segment, out MeshSurfaceResult surface, bool inverse = false)
        {
            return m_ActiveMesh.RaycastMesh(segment, out surface, inverse);
        }

        public bool TryCalcSurfaceBump(MeshSurfaceResult surface, out Vector3 bump, out Vector3 normal)
        {
            return m_ActiveMesh.CalcBump(surface, out bump, out normal);
        }
        */

        public override bool TryCalcSurfaceState(IShapeState shapeState, out ISurfaceState surfaceState, bool inverse = false)
        {
            if (!shapeState.ClosestSegment.HasLength)
            {
                surfaceState = null;
                return false;
            }

            MeshSurfaceResult info;
            if (!m_ActiveMesh.RaycastMesh(shapeState.ClosestSegment, out info, inverse))
            {
                surfaceState = null;
                return false;
            }

            var surfaceStateData = new SurfaceState(shapeState);

            surfaceStateData.Penetration = shapeState.ClosestSegment.Length;

            surfaceStateData.PointOnSurface = info.Point;
            surfaceStateData.TargetMaterial = info.Renderer.materials[info.SubMeshIndex];
            surfaceStateData.SubMeshIndex = info.SubMeshIndex;

            // HACK: Need correspond when texture is not exist
            Vector3 bump, normal;
            if (!m_ActiveMesh.CalcBump(info, out bump, out normal))
            {
                surfaceStateData.BumpVector = default(Vector3);
                surfaceStateData.NormalVector = default(Vector3);

                surfaceState = surfaceStateData;
                return true;
            }

            surfaceStateData.BumpVector = bump;
            surfaceStateData.NormalVector = normal;

            surfaceState = surfaceStateData;
            return true;
        }
    }
}