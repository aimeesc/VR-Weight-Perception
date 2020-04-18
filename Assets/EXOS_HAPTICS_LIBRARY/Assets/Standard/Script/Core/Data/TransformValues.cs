using System;
using UnityEngine;

namespace exiii.Unity
{
    [Serializable]
    public class TransformValues
    {
        #region Inspector

        [Header(nameof(TransformValues))]
        [SerializeField]
        private Vector3 m_Position;

        [SerializeField]
        private Vector3 m_Rotation;

        [SerializeField]
        private Vector3 m_Scale = Vector3.one;

        #endregion Inspector
        
        public TransformValues()
        {
            this.m_Position = Vector3.zero;
            this.m_Rotation = Vector3.zero;
            this.m_Scale = Vector3.one;
        }

        public TransformValues(TransformValues parameter)
        {
            this.m_Position = parameter.m_Position;
            this.m_Rotation = parameter.m_Rotation;
            this.m_Scale = parameter.m_Scale;
        }

        public void LocalsSaveFrom(Transform trans)
        {
            m_Position = trans.localPosition;
            m_Rotation = trans.localEulerAngles;
            m_Scale = trans.localScale;
        }

        public void ScalesSaveFrom(Transform trans)
        {
            m_Scale = trans.localScale;
        }

        public void LocalsLoadTo(Transform trans)
        {
            trans.localPosition = m_Position;
            trans.localEulerAngles = m_Rotation;
            trans.localScale = m_Scale;
        }
    }
}