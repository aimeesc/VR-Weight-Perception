namespace exiii.Unity
{
    /// <summary>
    /// Interface for build object in prefabuilder
    /// </summary>
    public interface IExosPrefab : IClassifier
    {
        /// <summary>
        /// Type of prefab
        /// </summary>
        EPrefabType PrefabType { get; }

        /// <summary>
        /// Function called at the timing of generating an object in the prefab builder
        /// </summary>
        /// <param name="root"></param>
        void BuildHierarchy(RootScript root);
    }
}