namespace exiii.Unity
{
    public interface ITouchManipulation : ICycleManipulation<ITouchManipulation>
    {
        IShapeStateSet ShapeStateSet { get; }
        ISurfaceStateSet SurfaceStateSet { get; }

        bool PenetratorIsDefault { get; }

        bool TryUpdateTouchState(IShapeContainer shape);

        bool TryUpdateSurfaceState(ISurfaceContainer surface);
    }
}