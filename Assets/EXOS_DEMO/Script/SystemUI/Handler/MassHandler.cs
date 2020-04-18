using System.Linq;
using UnityEngine;

namespace exiii.Unity.UI
{
    public class MassHandler : HapticsEditorHandler<float>
    {
        public override void SetValueToObject(float value)
        {
            if (TargetObjects == null) { return; }

            foreach (var obj in TargetObjects)
            {
                var rigidbody = obj.GetComponent<Rigidbody>();

                if (rigidbody != null) { rigidbody.mass = value; }
            }
        }

        public override void SetValueToObject()
        {
            SetValueToObject(m_ValueHolder.Value);
        }

        protected override void GetValueFromTargetObject(GameObject target)
        {
            var rigidbody = target.GetComponentInChildren<Rigidbody>();

            if (rigidbody != null) { m_ValueHolder.Value = rigidbody.mass; }
        }
    }
}