using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public class MeshRendererContainer : MeshContainerBase
    {
        #region Inspector

#pragma warning disable 414

        [SerializeField]
        [FormerlySerializedAs("meshFilter")]
        protected MeshFilter m_MeshFilter;

        [SerializeField]
        [FormerlySerializedAs("renderer")]
        protected Renderer m_Renderer;

#pragma warning restore 414

        #endregion Inspector

        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_MeshFilter == null) { m_MeshFilter = GetComponent<MeshFilter>(); }

            if (m_Renderer == null) { m_Renderer = GetComponent<Renderer>(); }
        }

        // Use this for initialization
        protected override void Awake()
        {
            base.Awake();

            if (m_MeshFilter == null || m_Renderer == null) { enabled = false; }

            exMeshes.Add(new ExMesh(m_MeshFilter.sharedMesh, m_Renderer, m_MeshFilter.transform));
        }

        //private void FixedUpdate()
        //{
        //    MeshRayDetector.TryBakeMesh();
        //}
    }
}