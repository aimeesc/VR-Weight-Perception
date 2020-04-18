using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace exiii.Unity
{
    public static class ExtensionPrefab
    {
        public static bool IsPrefab(this Object obj)
        {
#if UNITY_EDITOR
            return PrefabUtility.GetPrefabParent(obj) == null && PrefabUtility.GetPrefabObject(obj) != null;
#else
            return false;
#endif
        }
    }
}