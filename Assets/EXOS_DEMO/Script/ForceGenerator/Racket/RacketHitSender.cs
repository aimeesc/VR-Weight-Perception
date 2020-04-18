using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;

namespace exiii.Unity.Sample
{
    public class RacketHitSender : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField]
        private bool DrawDebugLine = false; 

        private void Start()
        {
            this.OnCollisionEnterAsObservable()
                .Subscribe(col =>
                {
                    var racketHitReciver = col.gameObject.GetComponent<IRacketHitReciver>();

                    if (racketHitReciver != null)
                    {
                        var segment = racketHitReciver.RacketHitSegment;

                        segment.InitialPoint = col.contacts.First().point;

                        racketHitReciver.RacketHitSegment = segment;
                    }
                });

            this.OnCollisionStayAsObservable()
                .Subscribe(col =>
                {
                    var racketHitReciver = col.gameObject.GetComponent<IRacketHitReciver>();

                    if (racketHitReciver != null)
                    {
                        var segment = racketHitReciver.RacketHitSegment;

                        segment.TerminalPoint = col.contacts.First().point;

                        racketHitReciver.RacketHitSegment = segment;
                        racketHitReciver.Hit();
                    }

                    if(DrawDebugLine)
                    {
                        foreach (var contact in col.contacts)
                        {
                            EHLDebug.DrawLine(Vector3.zero, contact.point, Color.red);
                        }
                    }
                });
        }
    }
}

