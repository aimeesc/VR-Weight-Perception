//========= Copyright 2016-2019, HTC Corporation. All rights reserved. ===========

using exiii.Unity.VIU.Utility;
using UnityEngine;

namespace exiii.Unity.VIU.PoseTracker
{
    public class PoseTracker : BasePoseTracker
    {
        public Transform target;

        protected virtual void LateUpdate()
        {
            TrackPose(new RigidPose(target, true), target.parent);
        }
    }
}