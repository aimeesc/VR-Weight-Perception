using exiii.Unity.EXOS;
using exiii.Unity.Linq;
using exiii.Unity.Rx;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity.UI
{
    public class Slider : MonoBehaviour, IValueHolder<float>
    {
        [SerializeField]
        private float m_InitialValue = 0.5f;

        [SerializeField]
        private float m_Min = 0;

        [SerializeField]
        private float m_Max = 1;

        [SerializeField, Unchangeable]
        private float m_Value;

        [SerializeField, Unchangeable]
        private float m_Ratio;

        public float Value
        {
            get { return m_Value; }

            set
            {
                m_Value = Mathf.Clamp(value, m_Min, m_Max);
                SetPosition(Ratio);

                m_OnValueChanged.OnNext(m_Value);
            }
        }

        private float Ratio
        {
            get { return m_Ratio = (m_Value - m_Min) / (m_Max - m_Min); }
        }

        private Subject<float> m_OnValueChanged = new Subject<float>();

        public IObservable<float> OnValueChanged => m_OnValueChanged;

        private ILinkTo<OrientedSegment> m_LinkToSgment;

        private Vector3 SegmentOffset => transform.position - m_LinkToSgment.Value.ClosestPointOnSegment(transform.position);

        private bool m_Enabled = false;

        private Transform m_Toucher;

        private Offset m_TouchOffset;
        
        private void SetPosition(Vector3 position)
        {
            transform.position = position + SegmentOffset;
        }

        private void SetPosition(float ratio)
        {
            var position = m_LinkToSgment.Value.PointOnSegment(ratio);

            SetPosition(position);
        }

        private void UpdateValue()
        {
            var target = m_TouchOffset.CalcOffsetAppliedTo(m_Toucher);

            float ratio;

            m_LinkToSgment.Value.ClosestPointOnSegment(target.position, out ratio, true);

            Value = m_Min + (m_Max - m_Min) * ratio;
        }

        private void Start()
        {
            m_LinkToSgment = GetComponentInParent<ILinkTo<OrientedSegment>>();

            if (m_LinkToSgment == null) { gameObject.SetActive(false); }

            Value = m_InitialValue;
        }

        public void Update()
        {
            if (m_Enabled) { UpdateValue(); }
        }

        public void SlideStart(GameObject target)
        {
            if (m_Enabled) { return; }

            m_Toucher = target.transform;

            m_TouchOffset = ExTransform.GetLocalOffset(m_Toucher, transform);

            m_Enabled = true;
        }

        public void SlideEnd()
        {
            m_Enabled = false;
        }
    }
}
