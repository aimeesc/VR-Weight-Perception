using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.Develop
{
    [CreateAssetMenu(fileName = "NewUICanvasSetting", menuName = "EXOS/UI/UICanvasSetting")]
    public class UICanvasSetting : ExScriptableObject
    {
        #region Inspector

        [SerializeField]
        [FormerlySerializedAs("DefaultIndex")]
        private int m_DefaultIndex;

        [SerializeField]
        [FormerlySerializedAs("Prefabs")]
        private ExMonoBehaviour[] m_Prefabs;

        #endregion Inspector

        private Canvas m_Canvas = null;
        private List<GameObject> m_UIList = null;

        public void Initialize(Canvas canvas)
        {
            m_Canvas = canvas;
            m_UIList = new List<GameObject>();

            // create default ui.
            CreateWidget(m_DefaultIndex);
        }

        // find index by class type.
        public int FindIndex<T>()
        {
            for (int i = 0; i < m_Prefabs.Length; ++i)
            {
                ExMonoBehaviour target = m_Prefabs[i];
                T targetScript = target.GetComponent<T>();
                if (targetScript != null)
                {
                    return i;
                }
            }
            return -1;
        }

        // create ui by type
        public void CreateWidget<T>()
        {
            // TODO:仮、重複チェック.
            if (m_Canvas.GetComponentInChildren<T>() != null)
            {
                return;
            }

            CreateWidget(FindIndex<T>());
        }
        
        // create ui by index
        public void CreateWidget(int index)
        {
            if (m_Prefabs == null || index < 0 || index >= m_Prefabs.Length) { return; }

            GameObject target = m_Prefabs[index].gameObject;
            GameObject target_ui = Instantiate(target) as GameObject;
            target_ui.transform.SetParent(m_Canvas.transform);
            target_ui.transform.localScale = Vector3.one;

            // add to created ui list.
            m_UIList.Add(target_ui);
        }

        // destory all ui.
        public void DestroyAllWidget()
        {
            foreach (GameObject gameObject in m_UIList)
            {
                Destroy(gameObject);
            }
            m_UIList.Clear();
        }

    }
}
