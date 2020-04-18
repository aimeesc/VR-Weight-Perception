using UnityEditor;
using UnityEngine;

namespace exiii.Unity
{
    public class ObjectPreLoader
    {
        private static string AssetName { get { return nameof(ResourcesContainer); } }

        // Hack: Disable Temporary
        // [InitializeOnLoadMethod]
        private static void PreLoader()
        {
            var playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
            SerializedObject serializedObject = new SerializedObject(playerSettings);

            var preloadedAssets = serializedObject.FindProperty("preloadedAssets");

            if (preloadedAssets == null || !preloadedAssets.isArray) { return; }

            int size = preloadedAssets.arraySize;

            for (int i = 0; i < size; i++)
            {
                var element = preloadedAssets.GetArrayElementAtIndex(i);
                var obj = element.objectReferenceValue;

                if (obj != null) { if (obj.name == AssetName) { return; } }
            }

            var container = Resources.Load<ResourcesContainer>(AssetName);

            if (container == null)
            {
                EditorUtility.DisplayDialog("EXOS_SDK", "SDK was installed.\nNeed restart Unity Editor.", "continue");
                return;
            }

            preloadedAssets.arraySize = size + 1;

            var serializedProperty = preloadedAssets.GetArrayElementAtIndex(size);

            serializedProperty.objectReferenceValue = Resources.Load<ResourcesContainer>(AssetName);

            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();

            Debug.LogWarning("[EXOS_SDK] PreLoader add the preload assets : " + AssetName);
        }
    }
}