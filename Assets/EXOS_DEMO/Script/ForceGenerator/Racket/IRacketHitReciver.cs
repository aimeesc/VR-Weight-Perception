using UnityEngine;

namespace exiii.Unity.Sample
{

    public interface IRacketHitReciver  {

        OrientedSegment RacketHitSegment { get; set; }

        void Hit();
        void Exit();
    }

}
