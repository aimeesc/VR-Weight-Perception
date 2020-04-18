using exiii.Extensions;
using exiii.Unity.Linq;
using System.Linq;
using UnityEngine;

namespace exiii.Unity.Develop
{
    public class KeyConfigText : PanelUserBase
    {
        private KeyboardEventBase<EControllerActions>[] m_ControllerEvents;
        private KeyboardEventBase<ESceneActions>[] m_SceneEvents;
        private KeyboardEventBase<ECameraActions>[] m_CameraEvents;
        private KeyboardEventBase<EDebugActions>[] m_DebugEvents;

        private void Start()
        {
            m_ControllerEvents = FindObjectsOfType<KeyboardControllerEventContainer>()
                .SelectMany(container => container)
                .ToArray();

            m_SceneEvents = FindObjectsOfType<KeyboardSceneEventContainer>()
                .SelectMany(container => container)
                .ToArray();

            m_CameraEvents = FindObjectsOfType<KeyboardCameraEventContainer>()
                .SelectMany(container => container)
                .ToArray();

            m_DebugEvents = FindObjectsOfType<KeyboardDebugEventContainer>()
                .SelectMany(container => container)
                .ToArray();
        }

        public void UpdateText()
        {
            Panel.ClearText();

            Panel.AddText("Controller\n".BoldTag());
            m_ControllerEvents.Foreach(x => Panel.AddText(x.KeyCode + " : " + x.Action + " : " + x.Description + "\n"));

            Panel.AddText("\n");

            Panel.AddText("Scene\n".BoldTag());
            m_SceneEvents.Foreach(x => Panel.AddText(x.KeyCode + " : " + x.Action + " : " + x.Description + "\n"));

            Panel.AddText("\n");

            Panel.AddText("Camera\n".BoldTag());
            m_CameraEvents.Foreach(x => Panel.AddText(x.KeyCode + " : " + x.Action + " : " + x.Description + "\n"));

            Panel.AddText("\n");

            Panel.AddText("Debug\n".BoldTag());
            m_DebugEvents.Foreach(x => Panel.AddText(x.KeyCode + " : " + x.Action + " : " + x.Description + "\n"));
        }
    }
}