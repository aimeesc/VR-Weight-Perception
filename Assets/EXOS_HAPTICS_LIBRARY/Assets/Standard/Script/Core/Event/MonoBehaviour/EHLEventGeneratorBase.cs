using exiii.Unity.Rx;
using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    /// Base class that generates various events handled by EXOS device
    /// </summary>
    public abstract class EHLEventGeneratorBase : MonoBehaviour
    {
        /// <summary>
        /// Class of Use event
        /// </summary>
        protected ExEventSet<Unit> UseEventGenerator { get; } = new ExEventSet<Unit>();

        /// <summary>
        /// Interface of Use event
        /// </summary>
        public IExEventSet<Unit> UseEvent { get { return UseEventGenerator; } }

        /// <summary>
        /// Class of Grab event
        /// </summary>
        protected ExEventSet<Unit> GrabEventGenerator { get; } = new ExEventSet<Unit>();

        /// <summary>
        /// Interface of Grab event
        /// </summary>
        public IExEventSet<Unit> GrabEvent { get { return GrabEventGenerator; } }

        /// <summary>
        /// Class of ThumStick event
        /// </summary>
        protected ExEventSet<Unit> ThumStickEventGenerator { get; } = new ExEventSet<Unit>();

        /// <summary>
        /// Interface of ThumStick event
        /// </summary>
        public IExEventSet<Unit> ThumStickEvent { get { return ThumStickEventGenerator; } }
    }
}