using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public abstract class ResourcesHolder<T> : ResourcesBase<T>
        where T : ResourcesHolder<T>
    {
        private static IEnumerable<T> s_Instances;

        protected static IEnumerable<T> Instances
        {
            get
            {
                if (s_Instances != null && s_Instances.Count() > 0)
                {
                    return s_Instances;
                }

                /*
                s_Instances = Resources.LoadAll<T>(AssetName);

                if (s_Instances != null && s_Instances.Count() > 0)
                {
                    Log();
                    return s_Instances;
                }
                */

                s_Instances = Resources.LoadAll<T>(string.Empty);

                if (s_Instances != null && s_Instances.Count() > 0)
                {
                    Log();
                    return s_Instances;
                }

                LogWarning();
                return null;
            }
        }

        public static bool IsExist
        {
            get { return Instances != null && Instances.Count() > 0; }
        }

        #region Log

        private static bool s_WarningDone = false;

        private static void Log()
        {
            Debug.Log($"[EXOS_SDK] {AssetName} : Instances is set");
        }

        private static void LogWarning()
        {
            if (s_WarningDone) { return; }

            Debug.LogWarning($"[EXOS_SDK] {AssetName} : Instance is not found.");

            s_WarningDone = true;
        }

        #endregion Log
    }
}