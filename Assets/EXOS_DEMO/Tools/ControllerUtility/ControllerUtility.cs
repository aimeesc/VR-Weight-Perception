using exiii.Extensions;
using exiii.Unity.EXOS;
using System.Linq;

namespace exiii.Unity.Develop
{
    public class ControllerUtility : PanelUserBase
    {
        private bool m_DeviceForceEnable = true;

        private void Start()
        {
            SetForceEnableAll();
        }

        private void SetForceEnable(ExosConnector connector)
        {
            connector.ForceEnabled = m_DeviceForceEnable;

            Panel.AddText($"{connector.name}\n");
        }

        public void SetForceEnableAll()
        {
            if (Panel == null) { return; }

            Panel.ClearText();
            Panel.AddText("DeviceForceDisabled\n".BoldTag());
            Panel.Enabled = !m_DeviceForceEnable;

            var connectors = FindObjectsOfType<ExosConnector>();

            if (connectors == null) { return; }

            connectors
                .CheckNull()
                .Where(x => x.gameObject.activeInHierarchy && x.enabled)
                .Foreach(x => SetForceEnable(x));
        }

        public void ChangeForceEnableAll()
        {
            m_DeviceForceEnable = !m_DeviceForceEnable;

            SetForceEnableAll();
        }
    }
}

