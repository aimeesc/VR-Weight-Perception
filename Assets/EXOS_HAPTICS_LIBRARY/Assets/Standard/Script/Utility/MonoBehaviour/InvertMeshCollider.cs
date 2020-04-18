using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    /// In Awake, flip the mesh set for this object
    /// </summary>
    public class InvertMeshCollider : MonoBehaviour
    {
        public void Awake()
        {
            MeshCollider collider = GetComponent<MeshCollider>();

            Mesh mesh = collider.sharedMesh.Copy();

            if (mesh == null) { return; }

            for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
            {
                var triangles = new Triangles(mesh.GetTriangles(subMeshIndex));

                triangles.InvertTrianglesAll();

                mesh.SetTriangles(triangles.Array, subMeshIndex);
            }

            mesh.RecalculateNormals();

            mesh.UploadMeshData(false);

            collider.sharedMesh = mesh;
        }
    }
}