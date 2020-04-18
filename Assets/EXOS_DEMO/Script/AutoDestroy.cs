using exiii.Unity.Rx;
using System;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class AutoDestroy : MonoBehaviour
    {
        [SerializeField]
        private float m_ExistTime = 3.0f;

        private void Start()
        {
            Observable.Timer(TimeSpan.FromSeconds(m_ExistTime))
                .Subscribe(_ => { Destroy(gameObject); })
                .AddTo(this);
        }
    }
}