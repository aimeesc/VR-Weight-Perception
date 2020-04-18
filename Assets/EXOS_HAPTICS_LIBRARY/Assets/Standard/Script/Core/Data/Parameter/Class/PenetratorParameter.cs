using UnityEngine;
using System.Collections;
using System;

namespace exiii.Unity
{
    [CreateAssetMenu(fileName = "PenetratorParameter", menuName = "EXOS/Parameter/Penetrator")]
    public class PenetratorParameter : ParameterAsset<PenetratorParameter>
    {
        [Header(nameof(PenetratorParameter))]
        [SerializeField, UnchangeableInPlaying]
        private float m_Size = 0.02f;
        public float Size { get { return m_Size; } }

        [SerializeField, UnchangeableInPlaying]
        private float m_Density = 1.0f;
        public float Density { get { return m_Density; } }

        [SerializeField, UnchangeableInPlaying]
        private bool m_Visible = true;
        public bool Visible { get { return m_Visible; } }

        [SerializeField, UnchangeableInPlaying]
        private bool m_UseToPenetration = true;
        public bool UseToPenetration { get { return m_UseToPenetration; } }        

        protected override void Reset()
        {
            base.Reset();

            this.m_Size = 0.02f;
            this.m_Density = 1.0f;
            this.m_Visible = true;
            this.m_UseToPenetration = true;
        }

        public override PenetratorParameter CreateCopy(UnityEngine.Object owner)
        {
            var instance = Instantiate(this);

            instance.Owner = owner;
            instance.IsOriginal = false;

            instance.m_Size = this.m_Size;
            instance.m_Density = this.m_Density;
            instance.m_Visible = this.m_Visible;
            instance.m_UseToPenetration = this.m_UseToPenetration;

            return instance;
        }
    }
}
