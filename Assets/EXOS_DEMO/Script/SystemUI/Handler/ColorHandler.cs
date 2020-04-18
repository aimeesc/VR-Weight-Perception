using UnityEngine;
using exiii.Extensions;

namespace exiii.Unity.UI
{
    public class ColorHandler : HapticsEditorHandler<float>
    {
        public override void SetValueToObject(float value)
        {
            if (TargetObjects == null) { return; }

            foreach (var obj in TargetObjects)
            {
                var renderers = obj.GetComponentsInChildren<Renderer>();

                if (renderers != null)
                {
                    renderers.Foreach(x => x.material.color = Color.HSVToRGB(value, 0.5f, 1));
                }
            }
        }

        public override void SetValueToObject()
        {
            SetValueToObject(m_ValueHolder.Value);
        }

        protected override void GetValueFromTargetObject(GameObject target)
        {
            var renderer = target.GetComponentInChildren<Renderer>();

            if (renderer != null && renderer.material != null && renderer.material.HasProperty("_Color"))
            {
                float H, S, V;

                Color.RGBToHSV(renderer.material.color, out H, out S, out V);

                m_ValueHolder.Value = H;
            }
        }
    }
}