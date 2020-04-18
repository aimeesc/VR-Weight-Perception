using exiii.Unity.Connection;
using System;
using UnityEditor;
using UnityEngine.Events;

namespace exiii.Unity.EXOS
{
    public static class ExosManager
    {
        public static UnityEvent ConnectionDispose
        {
            get
            {
                if (ConnectionManager.IsExist)
                {
                    return ConnectionManager.Instance.ConnectionDisposeEvent;
                }
                else
                {
                    throw new NullReferenceException("[EXOS_SDK] ConnectionManager is not exist");
                }
            }
        }
    }
}

