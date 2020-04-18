using exiii.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public class FingerMeta : ClassifierBase
    {
        #region

        [SerializeField]
        public FingerNames FingerName = FingerNames.Index01;

#pragma warning disable 414
        [SerializeField, Unchangeable]
        private FingerType m_FingerType;
#pragma warning restore 414

        [SerializeField]
        public PenetratorParameter PenetratorParameter;

        #endregion

        protected void OnValidate()
        {
            var finger = (int)FingerName;

            m_FingerType = (FingerType)(finger - finger % 10);
        }

        #region IExTag

        public override string ExTag
        {
            get { return exiii.Unity.ExTag.MakeTag(FingerName); }
        }

        #endregion
    }

    public enum FingerNames
    {
        Thumb01 = 01,
        Thumb02 = 02,
        Thumb03 = 03,
        Thumb03_end = 04,
        Index01 = 11,
        Index02 = 12,
        Index03 = 13,
        Index03_end = 14,
        Middle01 = 21,
        Middle02 = 22,
        Middle03 = 23,
        Middle03_end = 24,
        Ring01 = 31,
        Ring02 = 32,
        Ring03 = 33,
        Ring03_end = 34,
        Pinky01 = 41,
        Pinky02 = 42,
        Pinky03 = 43,
        Pinky03_end = 44
    }

    public enum FingerType
    {
        Thumb = 0,
        Index = 10,
        Middle = 20,
        Ring = 30,
        Pinky = 40,
    }
}
