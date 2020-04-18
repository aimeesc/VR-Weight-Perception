using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace exiii.Unity.Sample
{
    public class ScrewDriverSwitch : MonoBehaviour
    {
        public void SwitchOn()
        {
            Transform switchTransform = this.transform;
            Vector3 switchLocalPosition = switchTransform.localPosition;
            switchLocalPosition.x -= 0.006f;
            switchTransform.localPosition = switchLocalPosition;
        }

        public void SwitchOff()
        {
            Transform switchTransform = this.transform;
            Vector3 switchLocalPosition = switchTransform.localPosition;
            switchLocalPosition.x += 0.006f;
            switchTransform.localPosition = switchLocalPosition;
        }
    }
}
