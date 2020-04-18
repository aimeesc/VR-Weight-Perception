using exiii.Extensions;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 414

namespace exiii.Unity.Develop
{
    public class DemoCanvas : StaticAccessableMonoBehaviour<DemoCanvas>
    {
        #region Static

        public static bool TryGetCanvas(out Canvas canvas)
        {
            if (!IsExist)
            {
                canvas = null;
                return false;
            }

            canvas = Instance.m_Canvas;
            return (canvas != null);
        }

        public static bool TryGetTextPanel(EPanelRole role, out TextPanel panel)
        {
            if (!IsExist)
            {
                panel = null;
                return false;
            }

            panel = Instance.m_TextPanels.Where(x => x.Role == role).FirstOrDefault();
            return (panel != null);
        }

        #endregion Static

        #region Inspector

        [Header(nameof(DemoCanvas))]
        [SerializeField]
        private Canvas m_Canvas;

        [SerializeField]
        private TextPanel[] m_TextPanels;

        #endregion Inspector

        protected override void OnValidate()
        {
            m_TextPanels.Foreach(x => x.OnValidate());
        }
    }

    [Serializable]
    public class TextPanel
    {
        [SerializeField, HideInInspector]
        private string m_Name;

        [SerializeField]
        private EPanelRole m_Role;

        public EPanelRole Role => m_Role;

        [SerializeField]
        private GameObject m_Panel;

        [SerializeField]
        private Text m_Text;

        public void OnValidate()
        {
            m_Name = m_Role.EnumToString();
        }

        public bool Enabled
        {
            get { return m_Panel.activeInHierarchy; }
            set { m_Panel.SetActive(value); }
        }

        public string Text
        {
            get { return m_Text.text; }
            set { m_Text.text = value; }
        }

        public void AddText(string str)
        {
            m_Text.text += str;
        }

        public void ClearText()
        {
            m_Text.text = "";
        }
    }

    public enum EPanelRole
    {
        ForceEnable,
        KeyConfig,
        FocusCheck,
    }
}