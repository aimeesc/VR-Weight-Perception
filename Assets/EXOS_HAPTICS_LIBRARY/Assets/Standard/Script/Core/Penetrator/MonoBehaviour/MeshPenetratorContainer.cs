using exiii.Extensions;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace exiii.Unity
{
    public sealed class MeshPenetratorContainer : InteractorNode, IPenetratorContainer
    {
        #region Inspector

        [SerializeField]
        private SkinnedMeshRenderer m_SkinnedMesh;

        [SerializeField]
        private bool m_UseBakeMesh = true;

        [SerializeField]
        private Collider m_TouchCollider;

        [Header("Debug")]
        [SerializeField]
        private bool m_DrawPoints = true;

        [SerializeField]
        private int cut = 50;

        #endregion Inspector

        private Mesh m_SampleMesh;

        protected override void Awake()
        {
            base.Awake();

            m_SampleMesh = new Mesh();
        }

        public override void Initialize()
        {
            base.Initialize();

            m_TouchCollider = GetComponentInChildren<Collider>();
        }

        private void Update()
        {
            if (m_DrawPoints) { Penetrators.Foreach(x => EHLDebug.DrawSphere(x.Center, 0.005f, Color.white)); }
        }

        #region IPenetratorContainer

        // HACK: Need optimize
        public IReadOnlyCollection<IPenetrator> Penetrators
        {
            get
            {
                if (m_UseBakeMesh) { m_SkinnedMesh.BakeMesh(m_SampleMesh); }

                var vertices = m_SampleMesh.vertices;

                Transform trans = m_SkinnedMesh.transform;
                Bounds bounds = m_TouchCollider.bounds;

                return vertices
                    .Where((vector, index) => index % cut == 0)
                    .Select(vector => trans.TransformPoint(vector))
                    .Where(vector => bounds.Contains(vector))
                    .Select(vector => new VectorPointPenetrator(vector))
                    .ToArray();
            }
        }

        #endregion IPenetratorContainer
    }
}