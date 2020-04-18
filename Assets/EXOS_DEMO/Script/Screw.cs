using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity.Sample
{
    public class Screw : MonoBehaviour
    {
        private float m_fastenSoundSwitchThreshold = 1.0f;
        //private float screwSizeMultiplier = 1.2f;

        private bool m_IsScrewStuck = false;
        //private Vector3 screwSizeCache;

        public bool IsScrewStuck
        {
            set { this.m_IsScrewStuck = value; }
            get { return this.m_IsScrewStuck; }
        }

        public FastenableChecker fastenableChecker;
       

        public void FastenScrew()
        {
            if (fastenableChecker.IsFastenable)
            {
                m_IsScrewStuck = false;
                GameObject collidingScrew = GameObject.Find(fastenableChecker.ScrewName);
                Vector3 screwPostion = collidingScrew.transform.position;
                if (screwPostion.z > -0.14f)
                {
                    m_IsScrewStuck = true;
                    ScrewDriverRotator.Instance.IsScrewTouched = true;
                }
                if (!m_IsScrewStuck)
                {
                    Transform screwTransform = collidingScrew.transform;
                    screwTransform.Translate(0.0f, 0.0f, 0.0005f);
                    screwTransform.Rotate(0, 0, -10);

                    if (ScrewDriverSoundController.Instance.ScrewDriverSound.playClip.name != "fasten 1")
                        ScrewDriverSoundController.Instance.FastenPlay();
                }
                else
                {

                    if (ScrewDriverSoundController.Instance.ScrewDriverSound.playClip.name != "stuck")
                    {
                        ScrewDriverSoundController.Instance.StuckPlay();
                        //screwSizeCache = this.gameObject.GetComponent<BoxCollider>().size;
                        //this.gameObject.GetComponent<BoxCollider>().size = screwSizeCache * screwSizeMultiplier;
                    }
                }

            }

            if (!fastenableChecker.IsCollideScrew)
            {
                //this.gameObject.GetComponent<BoxCollider>().size = screwSizeCache;
                if (ScrewDriverSoundController.Instance.ScrewDriverSound.playClip.name == "stuck" && ScrewDriverSoundController.Instance.AudioProgress > m_fastenSoundSwitchThreshold)
                {
                    ScrewDriverSoundController.Instance.FastenPlay();
                }
            }
        }

        public void ResetScrewPosition()
        {
            GameObject[] screw = GameObject.FindGameObjectsWithTag("Screw");
            for (int i = 0; i < screw.Length; i++)
            {
                Transform screwTransform = screw[i].transform;
                Vector3 screwPosition = screwTransform.position;
                screwPosition.z = -0.2f;
                screwTransform.position = screwPosition;
            }
        }
    }

}
