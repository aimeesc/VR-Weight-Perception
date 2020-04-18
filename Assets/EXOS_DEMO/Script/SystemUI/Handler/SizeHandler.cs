using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity.UI
{
    public class SizeHandler : HapticsEditorHandler<float>
    {
        public override void SetValueToObject(float value)
        {
            if (TargetObjects == null) { return; }

            foreach (var obj in TargetObjects)
            {
                var transform = obj.GetComponent<Transform>();

                if (transform != null) { transform.localScale = Vector3.one * value; }
            }
        }

        public override void SetValueToObject()
		{
            SetValueToObject(m_ValueHolder.Value);
        }

        protected override void GetValueFromTargetObject(GameObject target)
        {
            var transform = target.transform;

            if (transform != null) { m_ValueHolder.Value = ((transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3); }
        }
    }
}