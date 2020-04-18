namespace exiii.Unity
{
    public interface ISurfaceContainer
    {
        void CalcSurfaceStateSet(IShapeStateSet shapeState, SurfaceStateSet surfaceStateSet, bool inverse = false);

        // HACK: It needs to split the mesh relationship
        //bool TryCalcSurfaceResult(OrientedSegment segment, out MeshSurfaceResult surface, bool inverse = false);
    }
}