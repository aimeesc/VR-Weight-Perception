using exiii.Unity.EXOS;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity.UI
{
    public class FloatPropertyHandler<TType> : HapticsEditorHandler<float>
    {
        [Header("UI")]
        [SerializeField]
        private int propertyIndex;

        [SerializeField, Unchangeable]
        private string propertyName;

        [SerializeField, Unchangeable]
        private string propertyType;

        // Private field
        private PropertyInfo property;

        protected void OnValidate()
        {
            Initialize();
        }

        protected void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            System.Type type = typeof(TType);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            properties = properties.Where(x => x.PropertyType.Equals(typeof(float))).ToArray();

            propertyIndex = Mathf.Clamp(propertyIndex, 0, properties.Length - 1);

            property = properties[propertyIndex];
            propertyName = property.Name;
            propertyType = property.PropertyType.ToString();
        }

        public override void SetValueToObject(float value)
        {
            if (TargetObjects == null) { return; }

            foreach (var obj in TargetObjects)
            {
                var target = obj.GetComponentInChildren<TType>();

                if (target != null && property.PropertyType.Equals(typeof(float)))
                {
                    property.SetValue(target, value, null);
                }
            }
        }

        public override void SetValueToObject()
        {
            SetValueToObject(m_ValueHolder.Value);
        }

        protected override void GetValueFromTargetObject(GameObject target)
        {
            if (target != null && property.PropertyType.Equals(typeof(float)))
            {
                var value = (float)property.GetValue(target, null);

                m_ValueHolder.Value = value;
            }
        }
    }
}