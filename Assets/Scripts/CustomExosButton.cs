using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace exiii.Unity.PhysicsUI
{
    public class CustomExosButton : ExMonoBehaviour
    {

        [Header("PhysicsUIButton")]

        [SerializeField]
        [FormerlySerializedAs("RapidFire")]
        private bool m_RapidFire = false;

        [SerializeField]
        [FormerlySerializedAs("IntervalDown")]
        private float m_IntervalDown = 0.0f;

        [SerializeField]
        [FormerlySerializedAs("IntervalUp")]
        private float m_IntervalUp = 0.0f;

        [SerializeField]
        [FormerlySerializedAs("BodyDisplay")]
        private MeshRenderer m_BodyDisplay = null;

        [SerializeField]
        [FormerlySerializedAs("Foundation")]
        private PhysicsUIButtonFoundation m_Foundation = null;

        [SerializeField]
        [FormerlySerializedAs("LabelRenderer")]
        private MeshRenderer m_LabelRenderer = null;

        [SerializeField]
        [FormerlySerializedAs("LabelTexture")]
        private Texture m_LabelTexture = null;

        [SerializeField]
        [FormerlySerializedAs("NormalColor")]
        private Color m_NormalColor = Color.white;

        [SerializeField]
        [FormerlySerializedAs("PressedColor")]
        private Color m_PressedColor = Color.yellow;

        [Header("UnityEvent")]

        [SerializeField]
        [FormerlySerializedAs("OnStart")]
        private UnityEvent m_OnStart = new UnityEvent();

        [SerializeField]
        [FormerlySerializedAs("OnUpdate")]
        private UnityEvent m_OnUpdate = new UnityEvent();

        [SerializeField]
        [FormerlySerializedAs("OnEnd")]
        private UnityEvent m_OnEnd = new UnityEvent();

        // property.
        public bool FromOrigin { get; set; } = true;

        // private.
        private float m_FromUp = 0.0f;
        private float m_FromDown = 0.0f;

        // on awake.
        protected override void Awake()
        {
            // init label texture.
            InitLabelTexture();

            base.Awake();
        }

        // update per frame.
        public void Update()
        {
            // update button state.
            UpdateButtonState();
        }

        // update button color.
        public void SetButtonColor(float ratio)
        {
            if (m_BodyDisplay == null) { return; }

            if (m_BodyDisplay.material.HasProperty("_Color"))
            {
                Color current = m_BodyDisplay.material.GetColor("_Color");
                Color col = Color.Lerp(m_NormalColor, m_PressedColor, ratio);
                col.a = current.a;
                m_BodyDisplay.material.SetColor("_Color", col);
            }
        }

        // init label texture.
        private void InitLabelTexture()
        {
            if (m_LabelRenderer == null) { return; }

            if (m_LabelTexture != null)
            {
                Material labelMaterial = m_LabelRenderer.material;
                if (labelMaterial == null) { return; }
                labelMaterial.SetTexture("_MainTex", m_LabelTexture);
            }
            else
            {
                m_LabelRenderer.gameObject.SetActive(false);
            }
        }

        // update button state.
        public void UpdateButtonState()
        {
            if (m_Foundation == null) { return; }

            if (m_Foundation.IsDown())
            {
                OnButtonDown();
            }
            if (m_Foundation.IsStay())
            {
                OnButtonStay();
            }
            if (m_Foundation.IsUp())
            {
                OnButtonUp();
            }

            // reset origin state.
            if (!FromOrigin && m_Foundation.IsOrigin())
            {
                FromOrigin = true;
            }

            // update delta.
            m_FromUp += Time.deltaTime;
            m_FromDown += Time.deltaTime;
        }

        // on button down.
        private void OnButtonDown()
        {
            if (!m_RapidFire && !FromOrigin) { return; }
            if (m_FromDown < m_IntervalDown) { return; }

            EHLDebug.Log($"{name} : ButtonDown ", this, "PhysicsUI");
            m_OnStart.Invoke();

            m_FromDown = 0.0f;
            Debug.Log("apertei o boão");

        }

        // on button stay.
        private void OnButtonStay()
        {
            if (!m_RapidFire && !FromOrigin) { return; }
            EHLDebug.Log($"{name} : ButtonStay ", this, "PhysicsUI");
            m_OnUpdate.Invoke();
        }

        // on button up.
        private void OnButtonUp()
        {
            if (!m_RapidFire && !FromOrigin) { return; }
            if (m_FromUp < m_IntervalUp) { return; }

            EHLDebug.Log($"{name} : ButtonUp ", this, "PhysicsUI");

            m_OnEnd.Invoke();

            m_FromUp = 0.0f;
            FromOrigin = false;
        }
    }
}
