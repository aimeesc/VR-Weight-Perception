using UnityEngine;

namespace exiii.Unity.Expantion
{
    /// <summary>
    /// Add MeshCollider to the object where MeshFilter is present
    /// </summary>
    public class AddMeshCollider : MonoBehaviour
    {
        [SerializeField]
        private bool m_AddExMesh;

        [SerializeField]
        private bool m_AddMeshCollider;

        public void Done()
        {
            {
                foreach (MeshFilter child in transform.GetComponentsInChildren<MeshFilter>())
                {
                    if (m_AddMeshCollider && child.GetComponent<MeshCollider>() == null)
                    {
                        child.gameObject.AddComponent<MeshCollider>();
                    }

                    if (m_AddExMesh && child.GetComponent<MeshRendererContainer>() == null)
                    {
                        child.gameObject.AddComponent<MeshRendererContainer>();
                    }

                    if (m_AddExMesh && child.GetComponent<ExMeshSurfaceContainer>() == null)
                    {
                        child.gameObject.AddComponent<ExMeshSurfaceContainer>();
                    }
                }
            }
        }
    }
}