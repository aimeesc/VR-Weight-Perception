using UnityEngine;

namespace exiii.Unity
{
    [CreateAssetMenu(fileName = "ExosDebugUnityLog", menuName = "EXOS/Editor/ExosDebug/ExosDebugUnityLog")]
    public class EHLDebugUnityLog : EHLDebug
    {
        protected override void InstanceLog(string str, UnityEngine.Object context)
        {
            Debug.Log(str, context);
        }

        protected override void InstanceLogWarning(string str, UnityEngine.Object context)
        {
            Debug.LogWarning(str, context);
        }

        protected override void InstanceLogError(string str, UnityEngine.Object context)
        {
            Debug.LogError(str, context);
        }

        protected override string InstanceGetTag(ExosLogSetting settings)
        {
            return $"[{settings.Type.ToString()}] ".ColorTag(settings.LogColor);
        }
    }
}