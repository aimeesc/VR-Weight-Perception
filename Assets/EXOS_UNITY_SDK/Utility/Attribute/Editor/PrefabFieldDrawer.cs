using UnityEditor;
using UnityEngine;

namespace exiii.Unity
{
    [CustomPropertyDrawer(typeof(PrefabFieldAttribute))]
    public class PrefabFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue != null)
            {
                var prefabType = PrefabUtility.GetPrefabType(property.objectReferenceValue);
                switch (prefabType)
                {
                    case PrefabType.Prefab:
                    case PrefabType.ModelPrefab:
                        break;

                    default:
                        // Prefab以外がアタッチされた場合アタッチを外す
                        property.objectReferenceValue = null;
                        break;
                }
            }

            label.text += " (Prefab Only)";
            EditorGUI.PropertyField(position, property, label);
        }
    }
}