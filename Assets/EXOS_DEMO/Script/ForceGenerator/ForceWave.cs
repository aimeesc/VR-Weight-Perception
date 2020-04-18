using exiii.Unity.EXOS;
using System;
using exiii.Unity.Rx;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class ForceWave : MonoBehaviour
    {
        [SerializeField] private float max = 0.5f;
        [SerializeField] private float Hz = 10f;
        [SerializeField] private float TimetoZero = 1.0f;

        [SerializeField] private Vector3 diretion;

        private bool isActive = false;

        private float deltaTime = 0.0f;

        public void Impact()
        {
            deltaTime = 0.0f;

            isActive = true;

            Observable.Timer(TimeSpan.FromSeconds(TimetoZero))
                .Subscribe(_ => { isActive = false; })
                .AddTo(this);
        }

        public void OnGenerate(IForceReceiver receiver, IState data)
        {
            if (isActive)
            {
                deltaTime += Time.deltaTime;

                Vector3 force = transform.TransformDirection(diretion) * Mathf.Cos(deltaTime * Hz * 2 * Mathf.PI) * max * ((TimetoZero - deltaTime) / TimetoZero);

                //Debug.Log("Wave : " + force.magnitude);

                receiver.AddForceRatio(transform.position, force);
            }
        }
    }
}