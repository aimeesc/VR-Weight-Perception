using exiii.Unity.SuperCharacterController;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity
{
    public class ColliderShapeContainer : ShapeContainerBase
    {
        #region Inspector

        [SerializeField]
        private Collider m_Collider;

        [SerializeField]
        private string[] m_IPenetrators;

        [SerializeField, Unchangeable]
        private bool isMesh = false;

        #endregion

        private Dictionary<IPenetrator, ContactPoint> m_PointDictionary = new Dictionary<IPenetrator, ContactPoint>();

        private Func<IPenetrator, ContactPoint, bool> CheckInternal;

        protected override void Start()
        {
            base.Start();

            if (m_Collider == null)
            {
                m_Collider = GetComponent<Collider>();

                if (m_Collider == null)
                {
                    UnityEngine.Debug.LogWarning("ColliderShapeContainer : Collider not found");
                    enabled = false;
                }
            }

            m_Collider.contactOffset = 0.001f;

            if (m_Collider is BoxCollider || m_Collider is SphereCollider || m_Collider is CapsuleCollider)
            {
                CheckInternal = CheckInternalWithClosestPoint;
                return;
            }

            var meshCollider = m_Collider as MeshCollider;

            if (meshCollider != null)
            {
                isMesh = true;

                if (meshCollider.convex)
                {
                    CheckInternal = CheckInternalWithClosestPoint;
                    return;
                }
                else
                {
                    CheckInternal = CheckInternalWithRaycast;
                    return;
                }
            }

            UnityEngine.Debug.LogWarning("ColliderShapeContainer : Collider type could not be determined.");
            enabled = false;
        }

        [Conditional(UnityDefineDirectives.UNITY_EDITOR)]
        private void Update()
        {
            m_IPenetrators = m_PointDictionary.Keys.Select(x => x.ToString()).ToArray();
        }

        protected override bool TryCalcPenetration(IPenetrator penetrator, out OrientedSegment penetration)
        {
            if (m_Collider == null)
            {
                penetration = default(OrientedSegment);
                return false;
            }

            PenetrationStatus status;

            if (TryCalcColliderPenetration(penetrator, out status))
            {
                return penetrator.TryCalcCorrection(status, out penetration);
            }

            if (penetrator.CorrectionType == ECorrectionType.Implicit && TryCalcColliderClosestPoint(penetrator, out status))
            {
                return penetrator.TryCalcCorrection(status, out penetration);
            }

            ContactPoint contact;

            if (!m_PointDictionary.TryGetValue(penetrator, out contact))
            {
                penetration = default(OrientedSegment);
                return false;
            }

            if (!CheckInternal(penetrator, contact))
            {
                contact.UpdatePoint(penetrator.Center);
            }

            penetration = new OrientedSegment(penetrator.Center, contact.FirstPoint);
            return true;
        }

        private bool TryCalcColliderPenetration(IPenetrator penetrator, out PenetrationStatus status)
        {
            Collider penetratorCollider;

            var hasCollider = penetrator.TryGetCollider(out penetratorCollider);

            if (!hasCollider || m_Collider == null)
            {
                status = null;
                return false;
            }

            Vector3 direction;
            float distance;

            var contact = Physics.ComputePenetration(penetratorCollider,
                                                        penetratorCollider.transform.position,
                                                        penetratorCollider.transform.rotation,
                                                        m_Collider,
                                                        m_Collider.transform.position,
                                                        m_Collider.transform.rotation,
                                                        out direction,
                                                        out distance);

            if (contact == false)
            {
                status = null;
                return false;
            }

            RaycastHit hit;

            var ray = new Ray(penetratorCollider.transform.position, -direction);

            ray.origin = ray.GetPoint(penetratorCollider.transform.lossyScale.x);
            ray.direction = -ray.direction;

            penetratorCollider.Raycast(ray, out hit, penetratorCollider.transform.lossyScale.x * 2);

            status = new PenetrationStatus(hit.point, hit.point + direction * distance);

            return true;
        }

        private bool TryCalcColliderClosestPoint(IPenetrator penetrator, out PenetrationStatus status)
        {
            if (m_Collider == null || isMesh)
            {
                status = null;
                return false;
            }

            Vector3 closest;
            bool isInternal;

            if (!SuperCollider.ClosestPointOnSurface(m_Collider, penetrator.Center, 0.0f, out closest, out isInternal))
            {
                status = null;
                return false;
            }

            status = new PenetrationStatus(penetrator.Center, closest, isInternal);
            return true;
        }

        private bool CheckInternalWithClosestPoint(IPenetrator penetrator, ContactPoint contact)
        {
            return m_Collider.ClosestPoint(penetrator.Center) == penetrator.Center;
        }

        // HACK: It needs correspond to the case of raycast hit twice or more
        private bool CheckInternalWithRaycast(IPenetrator penetrator, ContactPoint contact)
        {
            if (penetrator.Center == contact.FirstPoint) { return true; }

            var direction = penetrator.Center - contact.FirstPoint;
            var distance = direction.magnitude;
            var normalized = direction.normalized;

            var forword = new Ray(contact.FirstPoint, normalized);
            var reverse = new Ray(penetrator.Center, -normalized);

            RaycastHit forwordHit;
            RaycastHit reverseHit;

            var forwordCheck = m_Collider.Raycast(forword, out forwordHit, distance);
            var reverseCheck = !m_Collider.Raycast(reverse, out reverseHit, distance);

            return forwordCheck && reverseCheck;
        }

        private void OnTriggerEnter(Collider other)
        {
            var holder = other.GetComponent<PenetratorHolder>();

            if (holder == null || holder.Penetrator == null) { return; }

            if (!m_PointDictionary.ContainsKey(holder.Penetrator))
            {
                var contact = new ContactPoint(holder.Penetrator.Center);

                m_PointDictionary.Add(holder.Penetrator, contact);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var holder = other.GetComponent<PenetratorHolder>();

            if (holder == null || holder.Penetrator == null) { return; }

            ContactPoint contact;

            if (m_PointDictionary.TryGetValue(holder.Penetrator, out contact))
            {
                if (CheckInternal(holder.Penetrator, contact)) { return; }

                m_PointDictionary.Remove(holder.Penetrator);
            }
        }
    }

    internal class ContactPoint
    {
        public Vector3 FirstPoint { get; private set; }

        public ContactPoint(Vector3 first)
        {
            FirstPoint = first;
        }

        internal void UpdatePoint(Vector3 first)
        {
            FirstPoint = first;
        }
    }
}