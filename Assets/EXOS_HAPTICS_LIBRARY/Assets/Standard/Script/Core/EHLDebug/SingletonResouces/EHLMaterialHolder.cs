using UnityEngine;
using System;
using exiii.Unity.Rx;
using System.Collections.Generic;
using exiii.Extensions;

namespace exiii.Unity
{
    [CreateAssetMenu(fileName = "EHLMaterialHolder", menuName = "EXOS/Editor/ExosDebug/EHLMaterialHolder")]
    public class EHLMaterialHolder : SingletonResource<EHLMaterialHolder> 
	{
        #region Static

        static EHLMaterialHolder()
        {
            AssetName = nameof(EHLMaterialHolder);
        }

        private static List<Material> s_FlushMaterials = new List<Material>();

        public static Material InstantiateFlushMaterial()
        {
            if (!IsExist) { return null; }

            var material = Instantiate(Instance.m_FlushMaterial);

            s_FlushMaterials.Add(material);

            return material;
        }

        public static void StartFlush()
        {
            if (!IsExist) { return; }

            Instance.StartFlushInstance();
        }

        public static void EndFlush()
        {
            if (!IsExist) { return; }

            Instance.EndFlushInstance();
        }

        public static Material InstantiateColorGrid()
        {
            if (!IsExist) { return null; }

            return Instantiate(Instance.m_ColorGrid);
        }

        public static Material InstantiateTransparent()
        {
            if (!IsExist) { return null; }

            return Instantiate(Instance.m_Transparent);
        }

        public static PhysicMaterial InstantiateTools()
        {
            if (!IsExist) { return null; }

            return Instantiate(Instance.m_Tools);
        }

        #endregion

        #region Inspector

        //[Header(nameof(EHLMaterialHolder))]

        [Header(nameof(Material))]
        [SerializeField]
		private Material m_FlushMaterial;

        [SerializeField]
        private Color m_ColorForContact;

        [SerializeField]
        private float m_LerpInterval = 1;

        [SerializeField]
        private Material m_ColorGrid;

        [SerializeField]
        private Material m_Transparent;

        [Header(nameof(PhysicMaterial))]
        [SerializeField]
        private PhysicMaterial m_Tools;

        #endregion Inspector

        private IDisposable m_DisposableFlush;

        private void StartFlushInstance()
        {
            if (m_DisposableFlush != null) { return; }

            m_DisposableFlush = Observable
                .EveryUpdate()
                .Subscribe(_ => LerpColor(s_FlushMaterials, m_ColorForContact));

            Observable
                .OnceApplicationQuit()
                .Subscribe(_ => EndFlushInstance());
        }

        private void EndFlushInstance()
        {
            if (m_DisposableFlush == null) { return; }

            m_DisposableFlush.Dispose();
            m_DisposableFlush = null;
        }

        private void LerpColor(IEnumerable<Material> materials, Color flush)
        {
            materials.Foreach(x => LerpColor(x, flush));
        }

        private void LerpColor(Material material, Color flush)
        {
            material.color = Color.Lerp(flush, Color.black, Mathf.PingPong(Time.time, m_LerpInterval) / m_LerpInterval);
        }
    }
}
