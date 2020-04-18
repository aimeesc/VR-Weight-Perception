namespace exiii.Unity
{
    public interface ISizeEffector : IEffector
    {
        void OnChangeSizeRatio(ISizeState state);

        void OnResetSizeRatio();
    }
}