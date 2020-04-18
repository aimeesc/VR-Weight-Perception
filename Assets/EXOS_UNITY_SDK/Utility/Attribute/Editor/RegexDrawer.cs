using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace exiii.Unity
{
    [CustomPropertyDrawer(typeof(RegexAttribute))]
    public class RegexDrawer : PropertyDrawer
    {
        private const int HELP_HEIGHT = 30;
        private const int TEXT_HEIGHT = 16;

        private RegexAttribute RegexAttribute { get { return ((RegexAttribute)attribute); } }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            if (IsValid(prop))
            {
                return base.GetPropertyHeight(prop, label);
            }
            return base.GetPropertyHeight(prop, label) + HELP_HEIGHT;
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var textFieldPosition = position;
            textFieldPosition.height = TEXT_HEIGHT;
            DrawTextField(textFieldPosition, prop, label);

            var helpPosition = EditorGUI.IndentedRect(position);
            helpPosition.y += TEXT_HEIGHT;
            helpPosition.height = HELP_HEIGHT;
            DrawHelpBox(helpPosition, prop);
        }

        private void DrawTextField(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.TextField(position, label, prop.stringValue);
            if (EditorGUI.EndChangeCheck())
            {
                prop.stringValue = value;
            }
        }

        private void DrawHelpBox(Rect position, SerializedProperty prop)
        {
            if (IsValid(prop))
            {
                return;
            }

            EditorGUI.HelpBox(position, RegexAttribute.HelpMessage, MessageType.Error);
        }

        private bool IsValid(SerializedProperty prop)
        {
            return Regex.IsMatch(prop.stringValue, RegexAttribute.Pattern);
        }
    }
}