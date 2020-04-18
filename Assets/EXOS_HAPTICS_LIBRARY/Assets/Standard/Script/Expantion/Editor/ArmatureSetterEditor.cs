using UnityEditor;
using UnityEngine;

namespace exiii.Unity.Expantion
{
    [CustomEditor(typeof(ArmatureSetter))]
    public class ArmatureSetterEditor : Editor
    {
        /// <summary>
        /// InspectorのGUIを更新
        /// </summary>
        public override void OnInspectorGUI()
        {
            //元のInspector部分を表示
            base.OnInspectorGUI();

            //targetを変換して対象を取得
            var setter = target as ArmatureSetter;

            //PublicMethodを実行する用のボタン
            if (GUILayout.Button("Set Armature"))
            {
                setter.SetArmature();
            }
        }
    }
}