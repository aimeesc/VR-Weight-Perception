using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using exiii.Unity.Rx;
using System;

namespace exiii.Unity.PhysicsUI
{
    public class PhysicsUIButtonFoundation : ExMonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("ButtonBody")]
        private GameObject m_ButtonBody = null;

        [SerializeField, Unchangeable]
        [FormerlySerializedAs("BodyRange")]
        private Vector2 m_BodyRange = Vector2.zero;
        [Space(20)]

        [SerializeField]
        [FormerlySerializedAs("Threshold")]
        private float m_Threshold = 0.0f;

        // default position of body.
        private float m_BodyDefault = 0.0f;

        // difference from default.
        private float m_Difference = 0.0f;

        // difference ratio.
        private FloatReactiveProperty m_DifferenceRatio = null;

        // property.
        public float DifferenceRatio { get { return m_DifferenceRatio.Value; } }        

        // physics button state.
        private EPhysicUIButtonState m_ButtonState = 0;

        // on awake.
        protected override void Awake()
        {
            // set default value.
            m_BodyRange = new Vector2(float.MaxValue, float.MinValue);

            // get parent.
            PhysicsUIButton button = GetComponentInParent<PhysicsUIButton>();
            if (button != null)
            {
                // set reactive value.
                m_DifferenceRatio = new FloatReactiveProperty(0.0f);
                m_DifferenceRatio
                    .Subscribe(_ => button.SetButtonColor(_))
                    .AddTo(this);
            }

            // default position.
            if (m_ButtonBody != null)
            {
                m_BodyDefault = m_ButtonBody.transform.localPosition.y;
            }

            base.Awake();
        }

        // on update.
        public void Update()
        {
            // update difference.
            UpdateDifference();

            // update state.
            UpdateState();
        }

        // is exist on origin.
        public bool IsOrigin()
        {
            return (m_DifferenceRatio.Value < 0.01f);
        }

        // update current difference.
        private void UpdateDifference()
        {
            // calc difference.
            float BodyCurrent = m_ButtonBody.transform.localPosition.y;
            m_Difference = Mathf.Abs(BodyCurrent - m_BodyDefault);

            // update range.
            m_BodyRange.x = (m_Difference < m_BodyRange.x) ? m_Difference : m_BodyRange.x;
            m_BodyRange.y = (m_Difference > m_BodyRange.y) ? m_Difference : m_BodyRange.y;

            // update ratio.
            m_DifferenceRatio.Value = Mathf.Clamp01(m_Difference / m_Threshold);
        }

        // update current state.
        private void UpdateState()
        {
            bool OverThreshold = (m_Difference >= m_Threshold);
            EPhysicUIButtonState CurrentState = m_ButtonState;

            // check down.
            if (OverThreshold && !CurrentState.HasFlag(EPhysicUIButtonState.Stay))
            {
                m_ButtonState |= EPhysicUIButtonState.Down;
            }
            else
            {
                m_ButtonState &= ~EPhysicUIButtonState.Down;
            }

            // check stay.
            if (OverThreshold)
            {
                m_ButtonState |= EPhysicUIButtonState.Stay;
            }
            else
            {
                m_ButtonState &= ~EPhysicUIButtonState.Stay; 
            }

            // check up.
            if (!OverThreshold && CurrentState.HasFlag(EPhysicUIButtonState.Stay))
            {
                m_ButtonState |= EPhysicUIButtonState.Up;
            }
            else
            {
                m_ButtonState &= ~EPhysicUIButtonState.Up;
            }
        }        

        // is button down.
        public bool IsDown()
        {
            return m_ButtonState.HasFlag(EPhysicUIButtonState.Down);
        }

        // is button stay.
        public bool IsStay()
        {
            return m_ButtonState.HasFlag(EPhysicUIButtonState.Stay);
        }

        // is button up.
        public bool IsUp()
        {
            return m_ButtonState.HasFlag(EPhysicUIButtonState.Up);
        }
    }
}
