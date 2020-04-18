using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace exiii.Unity.Develop
{
    [CanEditMultipleObjects, CustomEditor(typeof(UIExButton), true)]
    public class UIExButtonEditor : ButtonEditor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_Image"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_IconNormal"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_IconHighlighted"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_IconPressed"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_IconDisabled"), true);
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
