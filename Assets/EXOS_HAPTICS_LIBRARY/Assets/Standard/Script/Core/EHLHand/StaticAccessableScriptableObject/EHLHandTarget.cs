using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace exiii.Unity
{
    [CreateAssetMenu(fileName = "EHLHandTarget", menuName = "EXOS/Hand/EHLHandTarget")]
    public class EHLHandTarget : StaticAccessableTarget<EHLHand>
    {
#if UNITY_EDITOR
        // default setting fosr resume.
        private static EHLHand s_DefaultTarget = null;
        private static bool s_AllowOverWrite = true;
#endif // UNITY_EDITOR.

        static EHLHandTarget()
        {
            AssetName = nameof(EHLHandTarget);

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        // resume default setting.
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (!IsExist) { return; }

            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    s_DefaultTarget = Instance.targetObject;
                    s_AllowOverWrite = Instance.AllowOverWrite;
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    Instance.targetObject = s_DefaultTarget;
                    Instance.AllowOverWrite = s_AllowOverWrite;
                    break;
            }
        }
#endif

        // change hand target.
        public static bool TryChangeTarget(EHLHand target)
        {
            if (!IsExist) { return false; }
            
            // check over write flag.
            if (!Instance.AllowOverWrite) { return false; }

            Instance.targetObject = target;
            return true;
        }

        // change allow overwrite.
        public static void ChangeAllowOverwirte(bool overwrite)
        {
            if (!IsExist) { return; }

            Instance.AllowOverWrite = overwrite;
        }
    }
}