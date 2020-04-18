using UnityEditor;
using UnityEngine;

namespace exiii.Unity.Expantion
{
    [CustomEditor(typeof(AddMeshCollider))]//拡張するクラスを指定
    public class AddMeshColliderEditor : Editor
    {
        /// <summary>
        /// InspectorのGUIを更新
        /// </summary>
        public override void OnInspectorGUI()
        {
            //元のInspector部分を表示
            base.OnInspectorGUI();

            //targetを変換して対象を取得
            AddMeshCollider addMeshCollider = target as AddMeshCollider;

            //PublicMethodを実行する用のボタン
            if (GUILayout.Button("AddCollider"))
            {
                addMeshCollider.Done();
            }
        }
    }
}