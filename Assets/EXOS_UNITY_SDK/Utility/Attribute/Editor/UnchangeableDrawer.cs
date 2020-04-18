using UnityEditor;
using UnityEngine;

namespace exiii.Unity
{
    [CustomPropertyDrawer(typeof(UnchangeableAttribute))]
    public sealed class UnchangeableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndDisabledGroup();
        }
    }
}