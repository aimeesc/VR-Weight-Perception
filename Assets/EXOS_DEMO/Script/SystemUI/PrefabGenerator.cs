using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public class PrefabGenerator : MonoBehaviour
    {
        [SerializeField, PrefabField]
        private GameObject m_TargetPrefab;

        public void Generate()
        {
            var obj = Instantiate(m_TargetPrefab, transform.position, transform.rotation);
            obj.name = m_TargetPrefab.name;
        }
    }
}


