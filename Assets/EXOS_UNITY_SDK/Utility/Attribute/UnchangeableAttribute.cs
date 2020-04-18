using System;
using UnityEngine;

namespace exiii.Unity
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public sealed class UnchangeableAttribute : PropertyAttribute
    {
        public UnchangeableAttribute()
        {
        }
    }
}