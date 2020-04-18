using exiii.Unity.EXOS;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public sealed class ForceTexture : InteractableNode, ISurfaceForceGenerator
    {
        [SerializeField]
        private float m_SurfaceGain = 1.0f;

        public void OnGenerate(IForceReceiver receiver, ISurfaceState state)
        {
            var surfaceVector = state.BumpVector - state.NormalVector * Vector3.Dot(state.NormalVector, state.BumpVector);

            receiver.AddForceRatio(state.PointOnSurface, surfaceVector * InteractableRoot.PhysicalProperties.Roughness * m_SurfaceGain);
        }
    }
}