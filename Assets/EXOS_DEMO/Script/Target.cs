using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using exiii.Extensions;
using System.Linq;

namespace exiii.Unity.Sample
{
    public class Target : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("Targets")]
        List<GameObject> m_Targets;

        private Dictionary<GameObject, Vector3> m_DefaultPosition = null;
        private Dictionary<GameObject, Quaternion> m_DefaultRotation = null;

        // on awake.
        public void Awake()
        {
            m_DefaultPosition = new Dictionary<GameObject, Vector3>();
            m_DefaultRotation = new Dictionary<GameObject, Quaternion>();

            // save default position.
            m_Targets.Foreach(x => m_DefaultPosition.Add(x, x.transform.position));
            m_Targets.Foreach(x => m_DefaultRotation.Add(x, x.transform.rotation));
        }

        // reset position.
        public void ResetPosition()
        {
            foreach (GameObject target in m_Targets)
            {
                target.transform.position = m_DefaultPosition[target];
                target.transform.rotation = m_DefaultRotation[target];

                Rigidbody rigid = target.GetComponent<Rigidbody>();

                if (rigid == null) { continue; }

                rigid.velocity = Vector3.zero;
                rigid.angularVelocity = Vector3.zero;
            }
        }

        [ContextMenu("SerchItems")]
        public void SerchItems()
        {
            var targets = FindObjectsOfType<InteractableRoot>().Select(x => x.gameObject);

            if (m_Targets != null)
            {
                m_Targets.AddRange(targets);
            }
            else
            {
                m_Targets = targets.ToList();
            }
        }

        [ContextMenu("RemoveNull")]
        public void RemoveNull()
        {
            m_Targets = m_Targets.CheckNull().ToList();
        }
    }
}
