using UnityEngine;

namespace exiii.Unity
{
    public class EnabledScope : GUI.Scope
    {
        private readonly bool buff_enabled;

        public EnabledScope(bool enabled)
        {
            this.buff_enabled = GUI.enabled;
            GUI.enabled = enabled;
        }

        protected override void CloseScope()
        {
            GUI.enabled = buff_enabled;
        }
    }

    public class ColorScope : GUI.Scope
    {
        private readonly Color color;

        public ColorScope(Color color)
        {
            this.color = GUI.backgroundColor;
            GUI.color = color;
        }

        protected override void CloseScope()
        {
            GUI.color = color;
        }
    }

    public class ContentColorScope : GUI.Scope
    {
        private readonly Color color;

        public ContentColorScope(Color color)
        {
            this.color = GUI.backgroundColor;
            GUI.contentColor = color;
        }

        protected override void CloseScope()
        {
            GUI.contentColor = color;
        }
    }

    public class BackgroundColorScope : GUI.Scope
    {
        private readonly Color color;

        public BackgroundColorScope(Color color)
        {
            this.color = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }

        protected override void CloseScope()
        {
            GUI.backgroundColor = color;
        }
    }
}