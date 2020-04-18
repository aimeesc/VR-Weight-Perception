﻿//========= Copyright 2016-2019, HTC Corporation. All rights reserved. ===========

using exiii.Unity.VIU.Utility;
using System;
using UnityEngine;

namespace exiii.Unity.VIU.PoseTracker
{
    public interface IPoseModifier
    {
        bool enabled { get; }
        int priority { get; set; }

        [Obsolete]
        void ModifyPose(ref Pose pose, Transform origin);

        void ModifyPose(ref RigidPose pose, Transform origin);
    }
}