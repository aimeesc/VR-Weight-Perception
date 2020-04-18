using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public static class ExtensionEnumerable
    {
        static System.Random s_Random = new System.Random();

        public static T RandomElementAt<T>(this IEnumerable<T> enumerables)
        {
            return enumerables.ElementAt(s_Random.Next(enumerables.Count()));
        }
    }
}
