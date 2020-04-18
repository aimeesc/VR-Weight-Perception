using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    /// Interface of objects that need to search references in prefab builders
    /// </summary>
    public interface INodeScript : IExMonoBehaviour
    {
        IRootScript RootScript { get; }

        GameObject RootGameObject { get; }

        bool InjectFinished { get; }

        /// <summary>
        /// A function called at the timing to obtain the reference of the object in the prefab builder
        /// </summary>
        /// <param name="root">Root object</param>
        void StartInjection(IRootScript root);

        void FinishInjection();
    }
}