using UnityEngine;

namespace exiii.Unity.SteamVR
{
    public class ViveDashboard : StaticAccessableMonoBehaviour<ViveDashboard>
    {
        #region Static

        public static bool TryGetEnabled(out bool value)
        {
            value = false;

            if (Valve.VR.OpenVR.Settings == null) { return false; }

            Valve.VR.EVRSettingsError er = new Valve.VR.EVRSettingsError();

            value = Valve.VR.OpenVR.Settings.GetBool(Valve.VR.OpenVR.k_pch_Dashboard_Section, Valve.VR.OpenVR.k_pch_Dashboard_EnableDashboard_Bool, ref er);

            Debug.Log("Success GetDashboard : " + value);

            return true;
        }

        public static bool TrySetEnabled(bool value)
        {
            if (Valve.VR.OpenVR.Settings == null) { return false; }

            Valve.VR.EVRSettingsError er = new Valve.VR.EVRSettingsError();

            Valve.VR.OpenVR.Settings.SetBool(
                Valve.VR.OpenVR.k_pch_Dashboard_Section,
                Valve.VR.OpenVR.k_pch_Dashboard_EnableDashboard_Bool,
                value, ref er
                );

            Valve.VR.OpenVR.Settings.Sync(false, ref er);

            Debug.Log("Success SetDashboard : " + value);

            return true;
        }

        #endregion Static

        #region Inspector

        [Header(nameof(ViveDashboard))]
        [SerializeField]
        private bool m_DisableDashboard;

        #endregion Inspector

        private bool m_Success;
        private bool m_DashboradEnableBuff;

        //StaticManagerBase
        public override void Initialize()
        {
            base.Initialize();

            m_Success = TryGetEnabled(out m_DashboradEnableBuff);

            if (m_Success && m_DisableDashboard)
            {
                TrySetEnabled(false);
            }
        }

        public override void Terminate()
        {
            base.Terminate();

            if (m_Success)
            {
                TrySetEnabled(m_DashboradEnableBuff);
            }
        }
    }
}

