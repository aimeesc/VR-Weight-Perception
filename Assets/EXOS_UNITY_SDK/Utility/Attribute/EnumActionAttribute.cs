using UnityEngine;
using System;

namespace exiii.Unity
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EnumActionAttribute : PropertyAttribute
    {
        public Type enumType;

        public EnumActionAttribute(Type enumType)
        {
            this.enumType = enumType;
        }
    }
}