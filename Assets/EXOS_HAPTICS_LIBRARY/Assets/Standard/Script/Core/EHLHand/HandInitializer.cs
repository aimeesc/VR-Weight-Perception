using exiii.Unity.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    public class HandInitializer : ExMonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("LayerSetting")]
        private LayerSetting[] m_LayerSetting = null;

        // on awake.
        protected override void Awake()
        {
            base.Awake();

            // Reset layer.
            ResetLayer();

            // build hand by setting.
            EHLHand.BuildHand(gameObject.layer);
        }

        // reset layer.
        private void ResetLayer()
        {
            // set layer.
            foreach (LayerSetting setting in m_LayerSetting)
            {
                if (setting.Prefab == null) { continue; }
                setting.Prefab.gameObject.DescendantsAndSelf()
                    .ForEach(_ => _.layer = setting.Layer);
            }
        }

        [System.Serializable]
        public class LayerSetting
        {
            public ExMonoBehaviour Prefab = null;
            public int Layer = 0;
        }
    }
}