namespace exiii.Unity
{
    public interface ISurfaceVisualEffector : IEffector
    {
        void AddEffect(ISurfaceState data);

        void ResetEffect(ISurfaceState data);
    }
}