using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public class TouchEffectGenerator :  ITouchForceGenerator, ITouchPositionGenerator
    {
        private IInteractableRoot m_Root;

        public TouchEffectGenerator(IInteractableRoot root)
        {
            m_Root = root;
        }

        public void OnGenerate(IForceReceiver receiver, IShapeStateSet shapeStateSet)
        {
            // Force
            var outputForce = CalcForceSegment(shapeStateSet, shapeStateSet.TouchForceParameter);

            receiver.AddForceRatio(outputForce);

            // RigidForce
            if (m_Root.Rigidbody != null)
            {
                var rigidForce = CalcRigidForceVector(outputForce.Vector, shapeStateSet.TouchForceParameter);

                m_Root.Rigidbody.AddForceAtPosition(rigidForce, outputForce.InitialPoint);
            }
        }

        private OrientedSegment CalcForceSegment(IShapeStateSet shapeStateSet, TouchForceParameter parameter)
        {
            var output = shapeStateSet.SummarizedOutput;

            // Force
            var ratio = output.Length / parameter.ForceMaxDistance;
            var gamma = Mathf.Pow(ratio, parameter.ForceGamma);
            
            var elasticityForce = output.VectorNormalized * (gamma * m_Root.PhysicalProperties.Elasticity + m_Root.PhysicalProperties.SurfaceHardness);

            //TODO: Correspond to viscosityForce
            var relativeVelocity = (m_Root.Rigidbody.GetRelativePointVelocity(output.InitialPoint) /*- shapeStateSet.Manipulator.PhysicsState.Velocity*/);
            var viscosityForce = relativeVelocity * m_Root.PhysicalProperties.Viscosity;

            return new OrientedSegment(output.InitialPoint, output.InitialPoint + elasticityForce + viscosityForce);
        }

        private Vector3 CalcRigidForceVector(Vector3 force, TouchForceParameter parameter)
        {
            return -force * parameter.RigidForceGain;
        }

        public void OnGenerate(IPositionReceiver receiver, IShapeStateSet shapeStateSet)
        {
            if (shapeStateSet == null) { return; }

            var outputPosition = CalcPositonRatioVector(shapeStateSet);

            receiver.AddPositionRatio(outputPosition);
        }

        private Vector3 CalcPositonRatioVector(IShapeStateSet shapeStateSet)
        {
            var output = shapeStateSet.SummarizedOutput;

            // Position
            return output.Vector * m_Root.PhysicalProperties.Elasticity;
        }
    }
}