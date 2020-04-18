using UnityEngine;

namespace exiii.Unity
{
    public class GraphAttribute : PropertyAttribute
    {
        public int count;

        public GraphAttribute(int count)
        {
            this.count = count;
        }
    }
}