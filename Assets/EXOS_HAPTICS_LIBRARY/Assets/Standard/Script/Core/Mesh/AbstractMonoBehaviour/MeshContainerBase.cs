using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public abstract class MeshContainerBase : NodeScript
    {
        [Header("MeshContainerBase")]
        [SerializeField]
        protected List<ExMesh> exMeshes = new List<ExMesh>();

        public IEnumerable<ExMesh> ExMeshes
        {
            get { return exMeshes; }
        }
    }
}