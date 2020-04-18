using UnityEngine;

namespace exiii.Unity
{
    [RequireComponent(typeof(LineRenderer))]
    public class ChainRenderer : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer line;

        public Transform target;

        private void OnValidate()
        {
            line = GetComponent<LineRenderer>();

            if (line && target)
            {
                line.positionCount = 2;
                line.useWorldSpace = false;
                line.SetPosition(1, transform.InverseTransformPoint(target.position));
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (line && target)
            {
                line.SetPosition(1, transform.InverseTransformPoint(target.position));
            }
        }
    }
}