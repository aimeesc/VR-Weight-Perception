using exiii.Extensions;
using exiii.Unity.EXOS;
using exiii.Unity.Rx;
using System.Linq;
using UnityEngine;

namespace exiii.Unity.UI
{
    public class HapticsObjectField : HapticsEditorBase
    {
        #region Inspector

        [SerializeField]
        private float m_GainAttraction = 300;

        [SerializeField]
        private float m_GainDeceleration = 3000;

        [SerializeField]
        private float m_GainRevolution = 0.03f;

        [SerializeField]
        private float m_GainDampingTorque = 100;

        [SerializeField]
        private float m_GainRepulsion = 30;

        #endregion

        protected void FixedUpdate()
        {
            m_SelectedObject
                .Where(x => !x.Interactable.ManipulationState.IsManipulated(EManipulationType.Grab) && !x.Interactable.ManipulationState.IsManipulated(EManipulationType.Grip))
                .Foreach(x => Attraction(x));
        }

        public override void OnEnter(InteractableRoot exosObject)
        {
            if (exosObject.IsPhysicalObject)
            {
                Select(exosObject);
            }
        }

        public override void OnExit(InteractableRoot exosObject)
        {
            Deselect(exosObject);
        }

        private void Attraction(SelectedObject obj)
        {
            // Attraction force to center
            Vector3 attraction = m_GainAttraction * (transform.position - obj.Rigidbody.position);
            Vector3 deceleration = m_GainDeceleration * -obj.Rigidbody.velocity * Time.fixedDeltaTime;
            Vector3 revolution = m_GainRevolution * Vector3.Cross(attraction, transform.up);

            Vector3 force = attraction + deceleration + revolution;
            obj.Rigidbody.AddForce(force, ForceMode.Acceleration);

            // Damp Angular velocity
            Vector3 torque = m_GainDampingTorque * -obj.Rigidbody.angularVelocity * Time.fixedDeltaTime;
            obj.Rigidbody.AddTorque(torque, ForceMode.Acceleration);

            // Repulsion force from others
            foreach (var other in m_SelectedObject.Where(x => x != obj))
            {
                Vector3 vector = obj.Rigidbody.position - other.gameObject.transform.position;

                if (vector.sqrMagnitude == 0) { continue; }

                Vector3 repulsion = vector * m_GainRepulsion / m_SelectedObject.Count / vector.magnitude;

                obj.Rigidbody.AddForce(repulsion, ForceMode.Acceleration);
            }
        }
    }
}