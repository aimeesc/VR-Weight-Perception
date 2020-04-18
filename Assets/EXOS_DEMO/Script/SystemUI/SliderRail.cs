using exiii.Unity.EXOS;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace exiii.Unity.UI
{
    [Serializable]
    public class SliderRail : MonoBehaviour, ILinkTo<OrientedSegment>
    {
        [SerializeField]
        private OrientedSegment m_LocalAxis;

        OrientedSegment ILinkTo<OrientedSegment>.Value => OrientedSegment.Transform(m_LocalAxis, transform);
    }
}