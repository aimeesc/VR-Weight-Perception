using UnityEngine;

namespace exiii.Unity
{
    [CreateAssetMenu(fileName = "ExosDebugTarget", menuName = "EXOS/Editor/ExosDebug/ExosDebugTarget")]
    public class EHLDebugTarget : StaticAccessableTarget<EHLDebug>
    {
        static EHLDebugTarget()
        {
            AssetName = nameof(EHLDebugTarget);
        }
    }
}