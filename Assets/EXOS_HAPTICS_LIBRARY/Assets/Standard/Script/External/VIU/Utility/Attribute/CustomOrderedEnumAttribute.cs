//========= Copyright 2016-2019, HTC Corporation. All rights reserved. ===========

using System;
using UnityEngine;

namespace exiii.Unity.VIU.Utility
{
    public class CustomOrderedEnumAttribute : PropertyAttribute
    {
        public Type overrideEnumType { get; private set; }

        public CustomOrderedEnumAttribute(Type overrideEnumType = null)
        {
            this.overrideEnumType = overrideEnumType;
        }
    }
}