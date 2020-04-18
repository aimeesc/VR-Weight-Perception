using exiii.Unity.Device;
using System.Collections.Generic;

namespace exiii.Unity.EXOS
{
    public interface IExosForceState : IForceState
    {
        IReadOnlyCollection<ExosForce> DirectForceList { get; }
        IReadOnlyDictionary<EAxisType, EForceMode> ForceModeDic { get; }
    }
}

