using UnityEngine;

namespace exiii.Unity
{
    public abstract class StaticAccessableScriptableObject<T> : ScriptableObject where T : StaticAccessableScriptableObject<T>
    {
        public static T Instance
        {
            get { return StaticAccessableTarget<T>.TargetObject; }
        }

        public static bool IsExist { get { return Instance != null; } }
    }
}