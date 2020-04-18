﻿//========= Copyright 2016-2019, HTC Corporation. All rights reserved. ===========

namespace exiii.Unity.VIU.PoseTracker
{
    public interface IPoseTracker
    {
        void AddModifier(IPoseModifier obj);

        bool RemoveModifier(IPoseModifier obj);
    }
}