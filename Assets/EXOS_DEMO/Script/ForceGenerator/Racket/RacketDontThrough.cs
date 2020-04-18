using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class RacketDontThrough : MonoBehaviour
    {

        [SerializeField]
        private Transform targetObj;

        private Vector3 oldPosition;
        private Quaternion oldRotation;

        private Vector3 boxSize;

        private RaycastHit hit;

        void Start()
        {
            boxSize = GetComponent<BoxCollider>().size;

            oldPosition = targetObj.position;
            oldRotation = targetObj.rotation;
        }

        void FixedUpdate()
        {
            transform.position = oldPosition;
            transform.rotation = oldRotation;

            var racketDiff = targetObj.position - oldPosition;

            var isHit = Physics.BoxCast(oldPosition, boxSize, racketDiff.normalized, out hit, transform.rotation, racketDiff.magnitude);

            if (isHit)
            {
                if (hit.collider.GetComponent<TamaDontThrough>())
                {
                    var diff = hit.point - oldPosition;
                    hit.transform.position += diff;
                }
            }

            // Debug.DrawLine(oldPosition, hit.point, Color.blue);

            oldPosition = targetObj.position;
            oldRotation = targetObj.rotation;
        }

        [Conditional(UnityDefineDirectives.UNITY_EDITOR)]
        void OnDrawGizmos()
        {
            var racketDiff = targetObj.position - oldPosition;

            var isHit = Physics.BoxCast(oldPosition, boxSize, racketDiff.normalized, out hit, transform.rotation, racketDiff.magnitude);
            if (isHit)
            {
                Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
                Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, boxSize);
            }
            else
            {
                Gizmos.DrawRay(transform.position, transform.forward * 100);
            }
        }
    }
}