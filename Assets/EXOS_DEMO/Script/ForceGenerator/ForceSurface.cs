using exiii.Unity.EXOS;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class ForceSurface : InteractableNode, ITouchForceGenerator
    {
        public enum EControlType
        {
            Speed,
            Depth,
        }

        [Header("SurfaceForce")]
        private const float depthGain = 10.0f;

        [SerializeField]
        protected EControlType ControlType = EControlType.Speed;

        private InteractableRoot m_Object;

        public override void StartInjection(IRootScript root)
        {
            base.StartInjection(root);

            m_Object = root.gameObject.GetComponent<InteractableRoot>();

            if (m_Object == null) { gameObject.SetActive(false); }
        }

        // TouchManipulation
        public void OnGenerate(IForceReceiver receiver, IShapeStateSet state)
        {
            // ITouchManipulation manipulation = receiver.Manipuration;

            Vector3 objectVelocity = m_Object.Rigidbody.GetPointVelocity(state.SummarizedOutput.InitialPoint);

            Vector3 relativeSpeed = receiver.TransformState.Velocity - objectVelocity;
            Vector3 surfaceSpeed = (relativeSpeed - state.SummarizedOutput.VectorNormalized * Vector3.Dot(state.SummarizedOutput.Vector, relativeSpeed));

            Vector3 forceVector = Vector3.zero;

            switch (ControlType)
            {
                case EControlType.Speed:
                    {
                        forceVector = (-surfaceSpeed * m_Object.PhysicalProperties.Friction);
                    }
                    break;

                case EControlType.Depth:
                    {
                        forceVector = (-surfaceSpeed * state.SummarizedOutput.Length * depthGain * m_Object.PhysicalProperties.Friction);
                    }
                    break;
            }

            receiver.AddForceRatio(state.SummarizedOutput.InitialPoint, forceVector);

            // Debug.DrawLine(data.ClosestSegment.InitialPoint, data.ClosestSegment.InitialPoint + forceVector, Color.red);
        }
    }
}