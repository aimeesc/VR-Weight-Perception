using exiii.Unity.EXOS;
using exiii.Unity.Rx;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.UI
{
    public class GravityHandler : HapticsEditorHandler<bool>, IValueHolder<bool>
    {
        bool m_Gravity = true;

        public bool Value
        {
            get { return m_Gravity; }
            set {
                m_Gravity = value;
                m_OnValueChanged.OnNext(m_Gravity);
            }
        }
        private Subject<bool> m_OnValueChanged = new Subject<bool>();
        public IObservable<bool> OnValueChanged => m_OnValueChanged;

        public void ToggleValue()
        {
            m_ValueHolder.Value = !m_ValueHolder.Value;
            SetValueToObject(m_ValueHolder.Value);
        }

        public override void SetValueToObject(bool value)
        {
            if (TargetObjects == null) { return; }

            foreach (var obj in TargetObjects)
            {
                var rigidbody = obj.GetComponent<Rigidbody>();

                if (rigidbody != null)
                {
                    rigidbody.useGravity = value;
                }
            }
        }

        public override void SetValueToObject()
        {
            SetValueToObject(m_ValueHolder.Value);
        }

        protected override void GetValueFromTargetObject(GameObject target)
        {
            var rigidbody = target.GetComponentInChildren<Rigidbody>();

            if (rigidbody != null) { m_ValueHolder.Value = rigidbody.useGravity; }
        }
    }
}
