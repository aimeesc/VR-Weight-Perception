namespace exiii.Unity
{
    /// <summary>
    /// Interface to detect contact of root object
    /// </summary>
    public interface IDetector : IMonoBehaviour
    {
        /// <summary>
        /// Function called at the start of contact
        /// </summary>
        /// <param name="component">Detected root object</param>
        void OnEnter(IDetectable component);

        /// <summary>
        /// Function called at the end of contact
        /// </summary>
        /// <param name="component">Detected root object</param>
        void OnExit(IDetectable component);

        /// <summary>
        /// Function called at the start of each contact
        /// </summary>
        /// <param name="component">Detected root object</param>
        void OnEachEnter(IDetectable component);

        /// <summary>
        /// Function called at the end of each contact
        /// </summary>
        /// <param name="component">Detected root object</param>
        void OnEachExit(IDetectable component);
    }

    /// <summary>
    /// Interface to detect contact of root object
    /// </summary>
    public interface IDetector<TComponent> : IMonoBehaviour
    {
        /// <summary>
        /// Function called at the start of contact
        /// </summary>
        /// <param name="component">Detected root object</param>
        void OnEnter(TComponent component);

        /// <summary>
        /// Function called at the end of contact
        /// </summary>
        /// <param name="component">Detected root object</param>
        void OnExit(TComponent component);

        /// <summary>
        /// Function called at the start of each contact
        /// </summary>
        /// <param name="component">Detected root object</param>
        void OnEachEnter(TComponent component);

        /// <summary>
        /// Function called at the end of each contact
        /// </summary>
        /// <param name="component">Detected root object</param>
        void OnEachExit(TComponent component);
    }
}