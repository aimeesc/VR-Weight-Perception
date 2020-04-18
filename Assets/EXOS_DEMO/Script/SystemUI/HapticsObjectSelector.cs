using exiii.Unity.Sample;
using exiii.Unity.EXOS;
using System;
using System.Linq;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using exiii.Unity.Linq;

#pragma warning disable 414

namespace exiii.Unity.UI
{
    public class HapticsObjectSelector : HapticsEditorBase
    {
        #region Inspector

        [SerializeField]
        private bool m_MultipleSelectable;

        [Header("Physics")]
        [SerializeField]
        private float m_GainAttractionForce;

        [SerializeField]
        private float m_GgainNearbyTargetVelocity;

        [SerializeField]
        private float m_NearbyDistanceLimit;

        [Header("Debug")]
        [SerializeField, Unchangeable]
        [FormerlySerializedAs("mediators")]
        private GameObject[] m_Casters;

        #endregion

        private ExTag m_Caster = new ExTag<ETag>(ETag.Caster);

        protected override void Start()
        {
            base.Start();

            m_Casters = RootGameObject.GetTagedGameObjectsInChildren(m_Caster);
        }

        protected void Update()
        {
            // m_Handlers.Foreach(x => x.SetValueToObject());
        }

        protected void FixedUpdate()
        {
            AccelerateToBase();
        }

        public override void OnEnter(InteractableRoot exosObject)
        {
            if (exosObject == null || !exosObject.IsPhysicalObject) { return; }

            if (m_MultipleSelectable)
            {
                if (m_SelectedObject.Contains(exosObject.gameObject))
                {
                    // Deselect(exosObject);
                }
                else
                {
                    Select(exosObject);
                }
            }
            else
            {
                if (m_SelectedObject.Contains(exosObject.gameObject))
                {
                    // Deselect(exosObject);
                }
                else
                {
                    DeselectAll();
                    Select(exosObject);
                }
            }
        }

        public override void OnExit(InteractableRoot exosObject)
        {
            
        }

        private void AccelerateToBase()
        {
            var rigid = GetComponent<Rigidbody>();

            Vector3 vectorToBase = Origin.transform.position - transform.position;

            float distance = vectorToBase.magnitude;

            Vector3 targetVelocity = Vector3.zero;

            if (distance < m_NearbyDistanceLimit)
            {
                targetVelocity = m_GgainNearbyTargetVelocity * vectorToBase;
            }
            else
            {
                targetVelocity = m_GgainNearbyTargetVelocity * m_NearbyDistanceLimit * vectorToBase.normalized;
            }

            Vector3 force = m_GainAttractionForce * (targetVelocity - rigid.velocity);
            rigid.AddForce(force, ForceMode.Acceleration);
        }
    }
}