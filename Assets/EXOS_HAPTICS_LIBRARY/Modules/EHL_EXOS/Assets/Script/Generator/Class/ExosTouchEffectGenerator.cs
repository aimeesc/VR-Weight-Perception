using UnityEngine;

namespace exiii.Unity.EXOS
{
    public class ExosTouchEffectGenerator : TouchEffectGenerator, IExosTouchForceGenerator
    {
        public ExosTouchEffectGenerator(IInteractableRoot root) : base (root)
        {
        }

        public void OnGenerate(IExosForceReceiver receiver, IShapeStateSet dataSet)
        {
        }
    }
}

