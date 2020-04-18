namespace exiii.Unity
{
    /// <summary>
    /// An interface for implementing a set of operations from start to finish
    /// </summary>
    public interface ICycleScript : IMonoBehaviour
    {
        /// <summary>
        /// Called at the start of operation
        /// </summary>
        void OnStart();

        /// <summary>
        /// Called on running Update
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// Called on running FixedUpdate
        /// </summary>
        void OnFixedUpdate();

        /// <summary>
        /// Called at the end of operation
        /// </summary>
        void OnEnd();
    }

    /// <summary>
    /// An interface for implementing a set of operations from start to finish with parameter
    /// </summary>
    /// <typeparam name="TData">Parameter Type</typeparam>
    public interface ICycleScript<TData> : IMonoBehaviour
    {
        /// <summary>
        /// Called at the start of operation with parameter
        /// </summary>
        void OnStart(TData data);

        /// <summary>
        /// Called on running Update with parameter
        /// </summary>
        void OnUpdate(TData data);

        /// <summary>
        /// Called on running FixedUpdate with parameter
        /// </summary>
        void OnFixedUpdate(TData data);

        /// <summary>
        /// Called at the end of operation with parameter
        /// </summary>
        void OnEnd(TData data);
    }
}