using exiii.Unity.Rx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity.UI
{
    abstract public class HapticsEditorHandler<TValueType> : HapticsEditorHandlerBase
    {
        [SerializeField, Unchangeable]
        private string ValueHolder;

        protected IValueHolder<TValueType> m_ValueHolder;

        public void Start()
        {
            var target = GetComponent<IValueHolder<TValueType>>();

            if (target == null)
            {
                gameObject.SetActive(false);
                return;
            }

            SetStateHolder(target);
        }

        public void SetStateHolder(IValueHolder<TValueType> stateHolder)
        {
            if (stateHolder == null) { return; }

            m_ValueHolder = stateHolder;

            ValueHolder = stateHolder.ToString();

            m_ValueHolder.OnValueChanged.Subscribe(x => SetValueToObject(x));
        }

        public abstract void SetValueToObject(TValueType value);

        protected abstract void GetValueFromTargetObject(GameObject target);

        public override void GetValueFromObject()
        {
            if (TargetObjects == null) { return; }

            if (TargetObjects.Count() > 0)
            {
                GetValueFromTargetObject(TargetObjects.ElementAt(0));
            }
        }

        public override void GetValueFromObject(GameObject obj)
        {
            if (TargetObjects == null || obj == null) { return; }

            if (TargetObjects.Contains(obj))
            {
                GetValueFromTargetObject(obj);
            }
        }
    }
}