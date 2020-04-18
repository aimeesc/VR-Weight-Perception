using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public abstract class SingletonResource<T> : ResourcesBase<T>
        where T : SingletonResource<T>
    {
        #region Static

        private static T s_Instance = null;

        protected static T Instance
        {
            get
            {
                if (s_Instance != null) { return s_Instance; }

                if (SearchTarget(AssetName)) { return s_Instance; }

                if (SearchTarget(string.Empty)) { return s_Instance; }

                LogWarning();
                return null;
            }
        }

        public static bool IsExist
        {
            get { return Instance != null; }
        }

        private static bool SearchTarget(string name)
        {
            if (name == null) { return false; }

            var targets = Resources.LoadAll<T>(name);

            if (targets.Length > 0)
            {
                s_Instance = targets.OrderBy(x => x.m_Order).First();

                Log();
                return true;
            }

            return false;
        }

        #region Log

        private static bool s_WarningDone = false;

        private static void Log()
        {
            Debug.Log($"[EXOS_SDK] {AssetName} : Instance is set", s_Instance);
        }

        private static void LogWarning()
        {
            if (s_WarningDone) { return; }

            Debug.LogWarning($"[EXOS_SDK] {AssetName} : Instance is not found.");

            s_WarningDone = true;
        }

        #endregion Log

        #endregion

        #region Instance

        [Header("SingletonResource")]
        //[TextArea]
        //public string m_Description = "Please don't change the asset name from the script name!";

        [Tooltip("Those with lower values are used preferentially. Default is 10")]
        [SerializeField]
        private int m_Order = 10;

        #endregion
    }
}