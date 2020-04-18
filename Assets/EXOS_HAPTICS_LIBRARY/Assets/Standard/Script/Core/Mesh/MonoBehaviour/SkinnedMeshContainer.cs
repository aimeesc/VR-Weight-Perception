using exiii.Extensions;
using UnityEngine;

namespace exiii.Unity
{
    public class SkinnedMeshContainer : MeshContainerBase
    {
        [SerializeField]
        private SkinnedMeshRenderer[] skinnedMeshRenderers;

        public override void StartInjection(IRootScript root)
        {
            base.StartInjection(root);

            skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            if (skinnedMeshRenderers == null) { enabled = false; }

            foreach (var mesh in skinnedMeshRenderers)
            {
                exMeshes.Add(new ExMesh(mesh));
            }
        }

        private void FixedUpdate()
        {
            ExMeshes.Foreach(detector => detector.TryBakeMesh());
        }
    }
}