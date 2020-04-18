using UnityEditor;
using UnityEngine;

namespace exiii.Unity
{
    [CustomPropertyDrawer(typeof(UnchangeableInPlayingAttribute))]
    public sealed class UnchangeableInPlayingDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndDisabledGroup();
        }
    }
}