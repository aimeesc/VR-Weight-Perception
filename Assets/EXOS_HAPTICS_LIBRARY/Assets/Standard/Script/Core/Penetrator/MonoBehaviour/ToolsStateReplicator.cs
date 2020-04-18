using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public class ToolsStateReplicator : StateReplicator
    {
        [SerializeField, Unchangeable, FormerlySerializedAs("MeshRenderer")]
        private MeshRenderer m_MeshRenderer = null;

        [SerializeField, Unchangeable, FormerlySerializedAs("SphereCollider")]
        private SphereCollider m_SphereCollider = null;

        private bool m_MeshRendererDefault = true;
        private bool m_SphereColliderDefault = true;

        // awake.
        protected override void Awake()
        {
            base.Awake();

            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_SphereCollider = GetComponent<SphereCollider>();

            m_MeshRendererDefault = m_MeshRenderer.enabled;
            m_SphereColliderDefault = m_SphereCollider.enabled;
        }

        // set default visible.
        public void SetDefaultVisible(bool visible)
        {
            m_MeshRendererDefault = visible;
        }

        // on change active.
        protected override void OnChangeActive(bool active)
        {
            if (!IsActiveObject()) { return; }

            // change renderer state.
            if (m_MeshRenderer != null)
            {
                m_MeshRenderer.enabled = (active) ? m_MeshRendererDefault : active;
            }

            // change collider state.
            if (m_SphereCollider != null)
            {
                m_SphereCollider.enabled = (active) ? m_SphereColliderDefault : active;
            }
        }        
    }
}