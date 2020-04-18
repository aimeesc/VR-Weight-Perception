using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    /// In Awake, flip the mesh set for this object
    /// </summary>
    public class InvertMeshFilter : MonoBehaviour
    {
        public void Awake()
        {
            MeshFilter filter = GetComponent<MeshFilter>();

            Mesh mesh = filter.sharedMesh.Copy();

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int index = triangles[i + 1];
                    triangles[i + 1] = triangles[i + 2];
                    triangles[i + 2] = index;
                }

                mesh.SetTriangles(triangles, m);
            }

            mesh.RecalculateNormals();

            mesh.UploadMeshData(false);

            filter.sharedMesh = mesh;
        }
    }
}