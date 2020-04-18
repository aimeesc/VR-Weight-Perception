using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Develop
{
    public class PanelUserBase : MonoBehaviour
    {
        [SerializeField]
        private EPanelRole m_PanelRole;

        private TextPanel m_Panel;

        public TextPanel Panel
        {
            get
            {
                if (m_Panel == null) { enabled = DemoCanvas.TryGetTextPanel(m_PanelRole, out m_Panel); }

                return m_Panel;
            }
        }
    }
}

