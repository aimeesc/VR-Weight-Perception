using UnityEngine;

namespace exiii.Unity
{
    [SelectionBase]
    public class TrackingOrigin : MonoBehaviour, ITrackingOrigin
    {
        [SerializeField]
        private TrackingSettings m_TrackingParameter;

        public TrackingSettings TrackingParameter
        {
            get { return m_TrackingParameter; }
        }

        public string ExTag
        {
            get { return m_TrackingParameter.TrackingType.ToString(); }
        }
    }
}