using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace exiii.Unity
{
    public static class ExtensionVector
    {
        public static Vector3 Sum(IEnumerable<Vector3> vectors)
        {
            var average = Vector3.zero;

            foreach (var v in vectors)
            {
                average += v;
            }

            return average;
        }

        public static Vector3 Average(IEnumerable<Vector3> vectors)
        {
            var average = Vector3.zero;
            var count = vectors.Count();

            foreach (var v in vectors)
            {
                average += v / count;
            }

            return average;
        }

        public static Vector2 Sum(IEnumerable<Vector2> vectors)
        {
            var average = Vector2.zero;

            foreach (var v in vectors)
            {
                average += v;
            }

            return average;
        }

        public static Vector2 Average(IEnumerable<Vector2> vectors)
        {
            var average = Vector2.zero;
            var count = vectors.Count();

            foreach (var v in vectors)
            {
                average += v / count;
            }

            return average;
        }

        // is contains value in range vector.
        public static bool ContainsRange(this Vector2 vector, float value)
        {
            if (vector.x >= vector.y) { return false; }

            return (value >= vector.x && value <= vector.y);
        }
    }
}