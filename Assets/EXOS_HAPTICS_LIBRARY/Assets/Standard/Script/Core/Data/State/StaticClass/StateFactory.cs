using UnityEngine;
using System.Collections;

namespace exiii.Unity
{
	public static class StateFactory
	{
        public static SurfaceState MakeSurfaceState(ITouchManipulator manipulator, MeshSurfaceResult result)
        {
            var state = new SurfaceState(manipulator)
            {
                PointOnSurface = result.Point,

                TargetMaterial = result.Renderer.materials[result.SubMeshIndex],

                SubMeshIndex = result.SubMeshIndex
            };

            return state;
        }

        public static SurfaceState MakeSurfaceState(ITouchState state, MeshSurfaceResult result)
        {
            return MakeSurfaceState(state.Manipulator, result);
        }
    }
}
