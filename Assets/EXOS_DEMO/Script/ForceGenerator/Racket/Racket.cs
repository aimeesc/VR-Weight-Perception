using exiii.Unity.EXOS;
using System;
using exiii.Unity.Rx.Triggers;
using exiii.Unity.Rx;
using UnityEngine;
using System.Linq;

namespace exiii.Unity.Sample
{
    public class Racket : ForceImpact
    {
        [Header("Racket")]

        [SerializeField]
        private float m_MassMax = 1.0f;

        [SerializeField]
        [Range(0.5f, 2.0f)]
        private float m_DurationGain = 1.0f;

        private void Start()
        {
            this.OnCollisionEnterAsObservable()
                .Where(col => col.gameObject.GetComponent<IRacketHitReciver>() != null)
                .Subscribe(col =>
                {
                    var racketHitReciver = col.gameObject.GetComponent<IRacketHitReciver>();

                    var segment = racketHitReciver.RacketHitSegment;

                    segment.InitialPoint = col.contacts.First().point;

                    racketHitReciver.RacketHitSegment = segment;
                });

            this.OnCollisionStayAsObservable()
                .Where(col => col.gameObject.GetComponent<IRacketHitReciver>() != null)
                .Subscribe(col =>
                {
                    var racketHitReciver = col.gameObject.GetComponent<IRacketHitReciver>();

                    var segment = racketHitReciver.RacketHitSegment;

                    segment.TerminalPoint = col.contacts.First().point;

                    racketHitReciver.RacketHitSegment = segment;
                    racketHitReciver.Hit();

                    foreach (var contact in col.contacts)
                    {
                        EHLDebug.DrawLine(Vector3.zero, contact.point, Color.red);
                    }

                });

            this.OnCollisionExitAsObservable()
                .Where(col => col.gameObject.GetComponent<IRacketHitReciver>() != null)
                .Subscribe(col => 
                {
                    var racketHitReciver = col.gameObject.GetComponent<IRacketHitReciver>();

                    racketHitReciver.Exit();
                });
        }

        private void Update()
        {
            EHLDebug.DrawLine(ForceOrigin, ForceOrigin + Direction * m_ForceGain, Color.red);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var rigid = collision.gameObject.GetComponent<Rigidbody>();

            if (rigid == null) { return; }

            var contactPoint = collision.contacts.First();

            ForceOrigin = contactPoint.point;
            Direction = contactPoint.normal.normalized;

            var targetHardness = collision.gameObject.GetComponent<IHardness>();

            if (targetHardness != null)
            {
                m_DeltaTime = targetHardness.Hardness * m_DurationGain;
            }

            var deltaVelocity = Vector3.Dot(Direction, collision.relativeVelocity.normalized);

            var force = Mathf.Clamp(rigid.mass, 0, m_MassMax) * (deltaVelocity / m_DeltaTime);

            Direction *= force;

            Impact();
        }
    }
}