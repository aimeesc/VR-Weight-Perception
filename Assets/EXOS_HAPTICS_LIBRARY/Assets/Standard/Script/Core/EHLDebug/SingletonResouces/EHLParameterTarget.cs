using UnityEngine;
using System.Collections;

namespace exiii.Unity
{
    [CreateAssetMenu(fileName = "EHLTarget", menuName = "EXOS/Editor/Parameter/EHLTarget")]
    public class EHLParameterTarget : StaticAccessableTarget<EHLParameterHolder> 
	{
        static EHLParameterTarget()
        {
            AssetName = nameof(EHLParameterTarget);
        }
	}
}
