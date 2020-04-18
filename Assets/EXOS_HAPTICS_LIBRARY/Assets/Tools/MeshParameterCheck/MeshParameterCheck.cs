using UnityEngine;
using exiii.Unity;

#pragma warning disable 414

namespace exiii.Unity
{
    public class MeshParameterCheck : MonoBehaviour
    {
        [SerializeField]
        MeshFilter meshFilter;

        [SerializeField]
        MeshCollider meshCollider;

        void OnValidate()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
        }

        [ContextMenu("MeshParameterCheck")]
        private void Check()
        {
            if (meshFilter != null)
            {
                Debug.Log("meshFilter.subMeshCount : ".ColorTag(Color.blue) + meshFilter.sharedMesh.subMeshCount);
                Debug.Log("meshCollider.subMeshCount : ".ColorTag(Color.blue) + meshFilter.sharedMesh.subMeshCount);
            }
        }

        void Awake()
        {
            Check();
        }
    }
}