using System;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    // tools build type.
    public enum EBuildType
    {
        All = 0,
        FromEnd = 1,
        FingerMeta = 2,
    }

    // tools size type.
    public enum ESizeType
    {
        Absolute = 0,
        LimitConflict = 1,
        Max = 2,
        FingerMeta = 3,
    }

    [CreateAssetMenu(fileName = "PenetratorBuildParameter", menuName = "EXOS/Parameter/PenetratorBuild")]
    public class PenetratorBuildParameter : ParameterAsset<PenetratorBuildParameter>
    {
        [SerializeField]
        private ETouchType m_PenetrationType = ETouchType.Tools;
        public ETouchType PenetrationType { get { return this.m_PenetrationType; } }

        [SerializeField]
        private EBuildType m_BuildType = EBuildType.All;
        public EBuildType BuildType { get { return this.m_BuildType; } }

        [SerializeField]
        private int m_BuildDepth = 3;
        public int BuildDepth { get { return this.m_BuildDepth; } }

        [SerializeField]
        private ESizeType m_SizeType = ESizeType.Absolute;
        public ESizeType SizeType { get { return this.m_SizeType; } }

        [SerializeField]
        private float m_TotalMass = 0.5f;
        public float TotalMass { get { return this.m_TotalMass; } }

        [SerializeField]
        private int m_Layer = 0;
        public int Layer { get { return this.m_Layer; } }

        [SerializeField]
        private bool m_Visible = true;
        public bool Visible { get { return this.m_Visible; } }

        //[SerializeField]
        //private PenetratorParameter m_PenetratorParameter = new PenetratorParameter(null);
        //public PenetratorParameter PenetratorParameter { get { return this.m_PenetratorParameter; } }

        protected override void Reset()
        {
            base.Reset();

            m_PenetrationType = ETouchType.Tools;
            m_BuildType = EBuildType.FingerMeta;
            m_BuildDepth = 3;
            m_SizeType = ESizeType.FingerMeta;
            m_Layer = 0;
            //m_PenetratorParameter = ;
            m_Visible = true;
        }

        public override PenetratorBuildParameter CreateCopy(UnityEngine.Object owner)
        {
            var instance = Instantiate(this);

            instance.Owner = owner;
            instance.IsOriginal = false;

            instance.m_PenetrationType = this.m_PenetrationType;
            instance.m_BuildType = this.m_BuildType;
            instance.m_BuildDepth = this.m_BuildDepth;
            instance.m_SizeType = this.m_SizeType;
            instance.m_Layer = this.m_Layer;
            //instance.m_PenetratorParameter = this.m_PenetratorParameter;
            instance.m_Visible = this.m_Visible;

            return instance;
        }
    }
}