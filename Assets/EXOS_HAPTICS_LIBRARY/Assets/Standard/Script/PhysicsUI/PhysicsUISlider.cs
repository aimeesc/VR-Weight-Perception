using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using exiii.Unity.Rx;

namespace exiii.Unity.PhysicsUI
{
    [Serializable]
    public class UnityEventSlider : UnityEvent<float> { }

    public class PhysicsUISlider : ExMonoBehaviour
    {
        [Header("PhysicsUISlider")]

        [SerializeField, Unchangeable, FormerlySerializedAs("Value")]
        private float m_Value = 0.0f;

        [SerializeField, FormerlySerializedAs("DefaultValue"), Range(0.0f, 1.0f)]
        private float m_DefaultValue = 0.5f;

        [SerializeField, FormerlySerializedAs("Handle")]
        private Transform m_Handle = null;

        [SerializeField, FormerlySerializedAs("TermL")]
        private Transform m_TermL = null;

        [SerializeField, FormerlySerializedAs("TermR")]
        private Transform m_TermR = null;

        [SerializeField, FormerlySerializedAs("OnValueChange")]
        private UnityEventSlider m_OnValueChange = new UnityEventSlider();

        public UnityEventSlider OnValueChangeEvent { get { return m_OnValueChange; } }

        private ReactiveProperty<float> m_SliderRatio = null;

        // on awake.
        protected override void Awake()
        {
            // init default position.
            SetValue(m_DefaultValue);

            // init reactive.
            m_SliderRatio = new ReactiveProperty<float>(CalcValue());
            m_SliderRatio.Subscribe(_ => OnValueChange(_)).AddTo(this);

            base.Awake();
        }

        // on update.
        protected void Update()
        {
            // update current ratio.
            m_SliderRatio.Value = CalcValue();
        }

        // on value change.
        private void OnValueChange(float value)
        {
            // change serialized value.
            m_Value = m_SliderRatio.Value;

            // invoke event.
            m_OnValueChange.Invoke(m_Value);
        }

        // set position value.
        protected void SetValue(float value)
        {
            if (m_Handle == null || m_TermL == null || m_TermR == null) { return; }

            // get length from game objects.
            float lengthRange = Mathf.Abs(m_TermR.localPosition.x - m_TermL.localPosition.x);
            float lengthHandle = m_Handle.localScale.x;
            float lengthTerm = m_TermL.localScale.x;

            // calc movable length.
            float movable = lengthRange - lengthTerm - lengthHandle;

            // calc position.
            float position = movable * ( value - 0.5f );

            Vector3 localPosition = m_Handle.localPosition;
            localPosition.x = position;
            m_Handle.localPosition = localPosition;
        }

        // calc value by position.
        protected float CalcValue()
        {
            if (m_Handle == null || m_TermL == null || m_TermR == null) { return 0.0f; }

            // get length from game objects.
            float lengthRange = Mathf.Abs(m_TermR.localPosition.x - m_TermL.localPosition.x);
            float lengthHandle = m_Handle.localScale.x;
            float lengthTerm = m_TermL.localScale.x;

            // calc movable length.
            float movable = lengthRange - lengthTerm - lengthHandle;

            // calc ratio.
            float wideRatio = m_Handle.localPosition.x / (movable * 0.5f);
            float ratio = Mathf.Clamp01(wideRatio * 0.5f + 0.5f);

            return ratio;
        }
    }
}
