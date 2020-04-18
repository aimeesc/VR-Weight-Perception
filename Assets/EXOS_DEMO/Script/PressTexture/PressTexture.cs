using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.Sample
{
    [RequireComponent(typeof(MeshFilter), typeof(Renderer))]
    public class PressTexture : InteractableNode, ISurfaceVisualEffector
    {
        [SerializeField]
        [FormerlySerializedAs("Size")]
        private Vector2Int m_Size;

        [SerializeField]
        [FormerlySerializedAs("Center")]
        private Vector2Int m_Center;

        [SerializeField]
        [FormerlySerializedAs("Softness")]
        [Range(0.0f, 1.0f)]
        private float m_Softness;

        [SerializeField]
        [FormerlySerializedAs("HeightOffset")]
        [Range(0.0f, 1.0f)]
        private float m_HieghtOffset;

        private List<int> m_EffectedIndex = null;
        private Dictionary<int, Texture2D> m_NormalMaps = null;
        private Dictionary<int, Texture2D> m_HeightMaps = null;

        // on awake.
        protected override void Awake()
        {
            base.Awake();

            m_EffectedIndex = new List<int>();
            m_NormalMaps = new Dictionary<int, Texture2D>();
            m_HeightMaps = new Dictionary<int, Texture2D>();
        }

        // on update.
        public void Update()
        {
            if (m_NormalMaps.Count == 0) { return; }

            List<int> deleteTarget = new List<int>();
            foreach (int index in m_NormalMaps.Keys)
            {
                if (m_EffectedIndex.Contains(index)) { continue; }

                deleteTarget.Add(index);
            }

            // delete effect.
            foreach (int index in deleteTarget)
            {
                ResetEffect(index);
            }

            m_EffectedIndex.Clear();
        }

        // add effect.
        public void AddEffect(ISurfaceState data)
        {
            // set target material.
            Material targetMaterial = FindMaterialByName(data.SubMeshIndex);
            if (targetMaterial == null) { return; }

            // calc height ratio.
            float heightRatio = data.Penetration / transform.parent.localScale.x;

            // set texture to new material.
            Texture2D normalTexture = FindNormalMap(data);
            Texture2D heightTexture = FindHeightMap(data);
            targetMaterial.SetTexture("_BumpMap", normalTexture);
            targetMaterial.SetTexture("_ParallaxMap", heightTexture);

            // set center.
            CalcCenter(data.PointOnSurface);

            // set ratio.
            targetMaterial.SetFloat("_Parallax", m_HieghtOffset);

            // calc.
            CalcNormalAndHeight(data, heightRatio);

            // log effected index.
            m_EffectedIndex.Add(data.SubMeshIndex);
        }

        // reset.
        public void ResetEffect(ISurfaceState data)
        {
            if (data == null) { return; }
            if (data.SubMeshIndex < 0) { return; }

            // set texture to new material.        
            Material targetMaterial = FindMaterialByName(data.SubMeshIndex);
            targetMaterial.SetTexture("_BumpMap", null);
            targetMaterial.SetTexture("_ParallaxMap", null);

            if (m_NormalMaps.ContainsKey(data.SubMeshIndex))
            {
                m_NormalMaps.Remove(data.SubMeshIndex);
            }

            if (m_HeightMaps.ContainsKey(data.SubMeshIndex))
            {
                m_HeightMaps.Remove(data.SubMeshIndex);
            }
        }

        // reset.
        public void ResetEffect(int submeshIndex)
        {
            if (submeshIndex < 0) { return; }

            // set texture to new material.        
            Material targetMaterial = FindMaterialByName(submeshIndex);
            targetMaterial.SetTexture("_BumpMap", null);
            targetMaterial.SetTexture("_ParallaxMap", null);

            if (m_NormalMaps.ContainsKey(submeshIndex))
            {
                m_NormalMaps.Remove(submeshIndex);
            }

            if (m_HeightMaps.ContainsKey(submeshIndex))
            {
                m_HeightMaps.Remove(submeshIndex);
            }
        }

        // calc center.
        protected void CalcCenter(Vector3 Point)
        {
            // calc local pos.
            Vector3 local_pos = transform.InverseTransformPoint(Point);
            Vector2 uv = CalcUV(local_pos);

            // calc new center.
            m_Center.x = Mathf.RoundToInt(m_Size.x * uv.x);
            m_Center.y = Mathf.RoundToInt(m_Size.y * uv.y);
        }

        // calc uv.
        protected Vector2 CalcUV(Vector3 local_pos)
        {
            MeshFilter filter = GetComponent<MeshFilter>();
            if (filter == null)
            {
                Debug.LogError("no filter");
                return Vector2.zero;
            }

            Mesh mesh = filter.mesh;
            if (mesh == null)
            {
                Debug.LogError("no mesh");
                return Vector2.zero;
            }

            for (var i = 0; i < mesh.triangles.Length; i += 3)
            {
                var index0 = i + 0;
                var index1 = i + 1;
                var index2 = i + 2;

                var p1 = mesh.vertices[mesh.triangles[index0]];
                var p2 = mesh.vertices[mesh.triangles[index1]];
                var p3 = mesh.vertices[mesh.triangles[index2]];
                var p = local_pos;

                var v1 = p2 - p1;
                var v2 = p3 - p1;
                var vp = p - p1;

                var nv = Vector3.Cross(v1, v2);
                var val = Vector3.Dot(nv, vp);
                var suc = -0.000001f < val && val < 0.000001f;

                if (!suc)
                {
                    continue;
                }
                else
                {
                    var a = Vector3.Cross(p1 - p3, p - p1).normalized;
                    var b = Vector3.Cross(p2 - p1, p - p2).normalized;
                    var c = Vector3.Cross(p3 - p2, p - p3).normalized;

                    var d_ab = Vector3.Dot(a, b);
                    var d_bc = Vector3.Dot(b, c);

                    suc = 0.999f < d_ab && 0.999f < d_bc;
                }

                if (!suc)
                {
                    continue;
                }
                else
                {
                    var uv1 = mesh.uv[mesh.triangles[index0]];
                    var uv2 = mesh.uv[mesh.triangles[index1]];
                    var uv3 = mesh.uv[mesh.triangles[index2]];

                    // calc perspective matrix.
                    Matrix4x4 mvp = Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix * transform.localToWorldMatrix;

                    // convert to projectionSpace.
                    Vector4 p1_p = mvp * new Vector4(p1.x, p1.y, p1.z, 1);
                    Vector4 p2_p = mvp * new Vector4(p2.x, p2.y, p2.z, 1);
                    Vector4 p3_p = mvp * new Vector4(p3.x, p3.y, p3.z, 1);
                    Vector4 p_p = mvp * new Vector4(p.x, p.y, p.z, 1);

                    // convert position.
                    Vector2 p1_n = new Vector2(p1_p.x, p1_p.y) / p1_p.w;
                    Vector2 p2_n = new Vector2(p2_p.x, p2_p.y) / p2_p.w;
                    Vector2 p3_n = new Vector2(p3_p.x, p3_p.y) / p3_p.w;
                    Vector2 p_n = new Vector2(p_p.x, p_p.y) / p_p.w;

                    // calc area amount.
                    var s = 0.5f * ((p2_n.x - p1_n.x) * (p3_n.y - p1_n.y) - (p2_n.y - p1_n.y) * (p3_n.x - p1_n.x));
                    var s1 = 0.5f * ((p3_n.x - p_n.x) * (p1_n.y - p_n.y) - (p3_n.y - p_n.y) * (p1_n.x - p_n.x));
                    var s2 = 0.5f * ((p1_n.x - p_n.x) * (p2_n.y - p_n.y) - (p1_n.y - p_n.y) * (p2_n.x - p_n.x));

                    // calc uv.
                    var u = s1 / s;
                    var v = s2 / s;
                    var w = 1 / ((1 - u - v) * 1 / p1_p.w + u * 1 / p2_p.w + v * 1 / p3_p.w);
                    var uv = w * ((1 - u - v) * uv1 / p1_p.w + u * uv2 / p2_p.w + v * uv3 / p3_p.w);

                    return uv;
                }
            }

            return Vector2.zero;
        }

        // calc normal and height.
        protected void CalcNormalAndHeight(ISurfaceState data, float heightRatio)
        {
            Color[] arHeight = new Color[m_Size.x * m_Size.y];
            Color[] arNormal = new Color[m_Size.x * m_Size.y];
            // Z = soft*(x-center.x)^2 + soft*(y-center.y)^2 - offset;

            float softness = m_Softness;

            for (int x = 0; x < m_Size.x; ++x)
            {
                for (int y = 0; y < m_Size.y; ++y)
                {
                    // calc phase.
                    int index = x + y * m_Size.x;

                    // calc height value.
                    float z = softness * Mathf.Pow(x - m_Center.x, 2) + softness * Mathf.Pow(y - m_Center.y, 2) - m_HieghtOffset * heightRatio;
                    arHeight[index] = Color.white * Mathf.Clamp01(-z);

                    // calc normal vector;
                    Vector3 normal = Vector3.zero;

                    if (z >= 0.0f)
                    {
                        normal.x = normal.y = normal.z = 0.5f;
                    }
                    else
                    {
                        // NOTE:偏微分を使ってnormal.zを求める部分が上手くいかない.
                        normal.x = -2.0f * softness * (x - m_Center.x);
                        normal.y = -2.0f * softness * (y - m_Center.y);
                        normal.z = -Mathf.Max(Mathf.Abs(normal.x), Mathf.Abs(normal.y));
                        normal.Normalize();
                    }

                    Color bump = Color.white;
                    bump.r = (normal.x + 1.0f) / 2.0f;
                    bump.g = (normal.y + 1.0f) / 2.0f;
                    bump.b = (normal.z + 1.0f) / 2.0f;
                    arNormal[index] = bump;
                }
            }

            // apply normal.
            Texture2D normalTexture = FindNormalMap(data);
            normalTexture.SetPixels(arNormal);
            normalTexture.Apply();

            // apply height.
            Texture2D heightTexture = FindHeightMap(data);
            heightTexture.SetPixels(arHeight);
            heightTexture.Apply();
        }

        // find material by name.
        protected Material FindMaterialByName(int index)
        {
            Material[] materials = GetComponent<MeshRenderer>()?.materials;
            if (index < 0 || index >= materials.Length)
            {
                return null;
            }
            return materials[index];
        }

        // find suitable normal map.
        protected Texture2D FindNormalMap(ISurfaceState data)
        {
            if (m_NormalMaps == null) { return null; }

            if (m_NormalMaps.ContainsKey(data.SubMeshIndex))
            {
                return m_NormalMaps[data.SubMeshIndex];
            }

            Texture2D tex = new Texture2D(m_Size.x, m_Size.y);
            tex.wrapMode = TextureWrapMode.Clamp;
            m_NormalMaps.Add(data.SubMeshIndex, tex);
            return tex;
        }

        // find suitable height map.
        protected Texture2D FindHeightMap(ISurfaceState data)
        {
            if (m_HeightMaps == null) { return null; }

            if (m_HeightMaps.ContainsKey(data.SubMeshIndex))
            {
                return m_HeightMaps[data.SubMeshIndex];
            }

            Texture2D tex = new Texture2D(m_Size.x, m_Size.y);
            tex.wrapMode = TextureWrapMode.Clamp;
            m_HeightMaps.Add(data.SubMeshIndex, tex);
            return tex;
        }
    }
}
