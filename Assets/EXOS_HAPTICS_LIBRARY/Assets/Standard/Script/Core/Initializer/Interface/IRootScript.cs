namespace exiii.Unity
{
    /// <summary>
    /// Root object in prefab builder
    /// </summary>
    public interface IRootScript : IExMonoBehaviour
    {
        bool RootInjected { get; }

        void BuildHierarchy();

        /// <summary>
        /// A function called at the timing to obtain the reference of the object in the prefab builder
        /// </summary>
        void RootInjection();

        // TRoot GetRootObject<TRoot>() where TRoot : IRootScript;
    }
}