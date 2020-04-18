using UnityEngine;

namespace exiii.Unity
{
    [RequireComponent(typeof(LineRenderer))]
    public class ConnectionLineDrawer : MonoBehaviour
    {
        [SerializeField]
        private Transform m_LineDrawTarget;

        private LineRenderer line;

        // Use this for initialization
        private void Start()
        {
            line = GetComponent<LineRenderer>();

            line.useWorldSpace = false;
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateLine();
        }

        private void FixedUpdate()
        {
            UpdateLine();
        }

        public void Initialize(Transform target)
        {
            m_LineDrawTarget = target;
        }

        private void UpdateLine()
        {
            if (line == null) { return; }

            line.SetPosition(0, transform.InverseTransformPoint(m_LineDrawTarget.position));
            line.SetPosition(1, transform.InverseTransformPoint(transform.position));
        }
    }
}