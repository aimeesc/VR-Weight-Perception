using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public abstract class SurfaceContainerBase : ShapeContainerBase, ISurfaceContainer
    {
        public virtual void CalcSurfaceStateSet(IShapeStateSet shapeStateSet, SurfaceStateSet surfaceStateSet, bool inverse = false)
        {
            foreach(var shapeState in shapeStateSet.Collection)
            {
                ISurfaceState surfaceState;

                if (TryCalcSurfaceState(shapeState, out surfaceState, inverse))
                {
                    surfaceStateSet.Add(surfaceState);
                }
            }
        }

        //public abstract bool TryCalcSurfaceResult(OrientedSegment segment, out MeshSurfaceResult surface, bool inverse = false);

        //public abstract bool TryCalcSurfaceBump(MeshSurfaceResult surface, out Vector3 bump, out Vector3 normal);

        public abstract bool TryCalcSurfaceState(IShapeState shapeState, out ISurfaceState surfaceState, bool inverse = false);
    }
}