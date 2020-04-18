using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.Develop
{
    public class UIRaycasterNavi
    {
        private static readonly int NaviMeshDiv = 20;
        private static readonly float NaviWidth = 0.1f;
        private static readonly float NaviAnimeSpeed = 10.0f;

        private Material m_NaviMaterial = null;
        private Transform m_FromTransform = null;
        private UIRaycasterInfo m_Target = null;
        private float m_UVOffsetTime = 0.0f;

        public bool IsActive { get; private set; } = false;

        // constructor.
        public UIRaycasterNavi(Material material)
        {
            m_NaviMaterial = material;
        }

        // create vertex.
        public void Create(Transform from, UIRaycasterInfo target)
        {
            if (IsActive) { return; }

            // save parent.
            m_FromTransform = from;
            m_Target = target;

            // ready mesh renderer.
            MeshRenderer renderer = m_FromTransform.GetOrAddComponent<MeshRenderer>();
            MeshFilter filter = m_FromTransform.GetOrAddComponent<MeshFilter>();

            // create parameters.
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var uvs = new List<Vector2>();
            CalcParameter(out vertices, out uvs, out indices);

            // set parameters.
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
            filter.mesh = mesh;

            // create material.
            renderer.material = m_NaviMaterial;

            IsActive = true;
        }

        // update vertex.
        public void Update(Transform from)
        {
            if (!IsActive) { return; }

            var vertices = new List<Vector3>();
            var indices = new List<int>();
            var uvs = new List<Vector2>();
            CalcParameter(out vertices, out uvs, out indices);

            MeshFilter filter = m_FromTransform.GetOrAddComponent<MeshFilter>();
            filter.mesh.SetVertices(vertices);
            filter.mesh.SetUVs(0, uvs);
            filter.mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

            m_UVOffsetTime += Time.deltaTime;
        }

        // destroy vertex.
        public void Destroy()
        {
            if (!IsActive) { return; }

            // remove filter.
            MeshFilter filter = m_FromTransform.GetOrAddComponent<MeshFilter>();
            if (filter != null)
            {
                filter.mesh = null;
            }

            IsActive = false;
        }

        // calc vertex and indices.
        protected void CalcParameter(out List<Vector3> vertices, out List<Vector2> uvs, out List<int> indices)
        {
            vertices = new List<Vector3>();
            indices = new List<int>();
            uvs = new List<Vector2>();

            Vector3 origin = Vector3.zero;
            Vector3 target = m_FromTransform.InverseTransformPoint(m_Target.HitPosition);
            float distance = Vector3.Distance(origin, target);
            float upwidth = distance * 0.1f;
            float uvOffset = m_UVOffsetTime * NaviAnimeSpeed - Mathf.Floor(m_UVOffsetTime);

            for (int i = 0; i < NaviMeshDiv; ++i)
            {
                float prevRatio = (float)i / (float)NaviMeshDiv;
                float nextRatio = (float)(i + 1) / (float)NaviMeshDiv;
                float prevUp = upwidth * Mathf.Sin(Mathf.PI * prevRatio);
                float nextUp = upwidth * Mathf.Sin(Mathf.PI * nextRatio);
                Vector3 prevUpVector = Vector3.up * prevUp;
                Vector3 nextUpVector = Vector3.up * nextUp;
                prevUpVector = m_FromTransform.InverseTransformDirection(prevUpVector);
                nextUpVector = m_FromTransform.InverseTransformDirection(nextUpVector);

                Vector3 leftBottom = Vector3.Lerp(origin, target, prevRatio);
                leftBottom += prevUpVector;

                Vector3 leftTop = Vector3.Lerp(origin, target, nextRatio);
                leftTop += nextUpVector;

                Vector3 rightBottom = Vector3.Lerp(origin, target, prevRatio);
                rightBottom.x += NaviWidth;
                rightBottom += prevUpVector;

                Vector3 rightTop = Vector3.Lerp(origin, target, nextRatio);
                rightTop.x += NaviWidth;
                rightTop += nextUpVector;

                vertices.Add(leftBottom);
                vertices.Add(rightBottom);
                vertices.Add(leftTop);
                vertices.Add(rightTop);

                uvs.Add(new Vector2(0.0f, NaviMeshDiv * prevRatio - uvOffset));
                uvs.Add(new Vector2(1.0f, NaviMeshDiv * prevRatio - uvOffset));
                uvs.Add(new Vector2(0.0f, NaviMeshDiv * nextRatio - uvOffset));
                uvs.Add(new Vector2(1.0f, NaviMeshDiv * nextRatio - uvOffset));

                indices.Add(2 + i * 4);
                indices.Add(1 + i * 4);
                indices.Add(0 + i * 4);
                indices.Add(2 + i * 4);
                indices.Add(3 + i * 4);
                indices.Add(1 + i * 4);

                indices.Add(0 + i * 4);
                indices.Add(1 + i * 4);
                indices.Add(2 + i * 4);
                indices.Add(1 + i * 4);
                indices.Add(3 + i * 4);
                indices.Add(2 + i * 4);
            }            
        }
    }
}