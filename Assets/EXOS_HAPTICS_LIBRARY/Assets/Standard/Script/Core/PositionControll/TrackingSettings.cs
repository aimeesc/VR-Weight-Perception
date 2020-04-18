using System;
using UnityEngine;

namespace exiii.Unity
{
    [Serializable]
    public class TrackingSettings
    {
        #region Inspector

        [Header(nameof(TrackingSettings))]
        [SerializeField]
        private ETrackingType m_TrackingType = ETrackingType.Controller;

        public ETrackingType TrackingType => m_TrackingType;

        [SerializeField]
        private ETrackingPositionType m_TrackingPosition = ETrackingPositionType.Palm;

        public ETrackingPositionType TrackingPosition => m_TrackingPosition;

        [SerializeField]
        private ELRType m_LRType = ELRType.None;

        public ELRType LRType => m_LRType;

        #endregion Inspector

        public TrackingSettings()
        {
            this.m_TrackingType = ETrackingType.Controller;
            this.m_TrackingPosition = ETrackingPositionType.Palm;
            this.m_LRType = ELRType.None;
        }

        public TrackingSettings(TrackingSettings parameter)
        {
            this.m_TrackingType = parameter.TrackingType;
            this.m_TrackingPosition = parameter.TrackingPosition;
            this.m_LRType = parameter.LRType;
        }

        public override bool Equals(object obj)
        {
            var settings = obj as TrackingSettings;
            return settings != null &&
                   m_TrackingType == settings.m_TrackingType &&
                   m_TrackingPosition == settings.m_TrackingPosition &&
                   m_LRType == settings.m_LRType;
        }

        public override int GetHashCode()
        {
            var hashCode = -1281647634;
            hashCode = hashCode * -1521134295 + m_TrackingType.GetHashCode();
            hashCode = hashCode * -1521134295 + m_TrackingPosition.GetHashCode();
            hashCode = hashCode * -1521134295 + m_LRType.GetHashCode();
            return hashCode;
        }
    }
}