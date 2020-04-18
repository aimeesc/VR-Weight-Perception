using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 414

namespace exiii.Unity
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField, Unchangeable]
        private int FPS;

        [SerializeField, Unchangeable]
        private int FFPS;

        private void Update()
        {
            FPS = (int) (1 / Time.deltaTime);
        }

        private void FixedUpdate()
        {
            FFPS = (int)(1 / Time.fixedDeltaTime);
        }
    }
}

