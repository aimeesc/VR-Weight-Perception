using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace exiii.Unity
{
    [CustomEditor(typeof(TutorialTextHandler))]
    public class TutorialTextStepEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var targetComponent = target as TutorialTextHandler;

            EditorGUILayout.LabelField("Step");
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Back"))
                {
                    targetComponent.TutorialStep(TutorialStepEnum.Back);
                }

                if (GUILayout.Button("Next"))
                {
                    targetComponent.TutorialStep(TutorialStepEnum.Next);
                }
            }
            EditorGUILayout.EndHorizontal();

            base.OnInspectorGUI();
        }
    }
}