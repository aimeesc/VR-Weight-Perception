using exiii.Unity.EXOS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity.UI
{
    public class PhysicalPropertiesHandler : HapticsEditorHandler<float>
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
            System.Type type = typeof(PhysicalProperties);
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

            foreach (var root in TargetObjects)
            {
                var holder = root.GetComponentInChildren<ILinkTo<PhysicalProperties>>();

                if (holder == null)
                {
                    gameObject.SetActive(false);
                    return;
                }

                if (holder.Value != null && property.PropertyType.Equals(typeof(float)))
                {
                    property.SetValue(holder.Value, value, null);
                }
            }
        }

        public override void SetValueToObject()
        {
            SetValueToObject(m_ValueHolder.Value);
        }

        protected override void GetValueFromTargetObject(GameObject target)
        {
            var holder = target.GetComponentInChildren<ILinkTo<PhysicalProperties>>();

            if (holder == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (holder.Value != null && property.PropertyType.Equals(typeof(float)))
            {
                var value = (float)property.GetValue(holder.Value, null);

                m_ValueHolder.Value = value;
            }
        }
    }
}