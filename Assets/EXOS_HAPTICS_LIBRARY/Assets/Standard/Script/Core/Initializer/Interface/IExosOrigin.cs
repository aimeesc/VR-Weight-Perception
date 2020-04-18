namespace exiii.Unity
{
    /// <summary>
    /// Interface for position reference object in prefabuilder
    /// </summary>
    public interface IExosOrigin : IClassifier
    {
        /// <summary>
        /// Type of prefab
        /// </summary>
        EPrefabType PrefabType { get; }
    }
}