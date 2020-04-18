//========= Copyright 2016-2019, HTC Corporation. All rights reserved. ===========

using System;
using UnityEngine;

namespace exiii.Unity.VIU.Utility
{
    public class FlagsFromEnumAttribute : PropertyAttribute
    {
        public Type EnumType { get; private set; }

        public FlagsFromEnumAttribute(Type enumType)
        {
            EnumType = enumType;
        }
    }
}