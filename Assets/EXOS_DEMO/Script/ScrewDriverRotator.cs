using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace exiii.Unity.Sample
{
    public class ScrewDriverRotator : MonoBehaviour
    {
        public static ScrewDriverRotator Instance;
        public bool IsScrewTouched = false;
        private void Start()
        {
            Instance = this;
        }

        public void RotateDriver()
        {
            if (!IsScrewTouched)
            {
                this.transform.Rotate(-10, 0, 0);
            }
        }
    }
}
