using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.Sample
{
    public class ScrewDriverSoundController : MonoBehaviour
    {

        public static ScrewDriverSoundController Instance;

        #region AudioClip

        [SerializeField]
        [FormerlySerializedAs("AudioClipStart")]
        private AudioClip m_Begin = null;

        [SerializeField]
        [FormerlySerializedAs("AudioClipEnd")]
        private AudioClip m_End = null;

        [SerializeField]
        [FormerlySerializedAs("AudioClipFasten")]
        private AudioClip m_Fasten = null;

        [SerializeField]
        [FormerlySerializedAs("AudioClipStuck")]
        private AudioClip m_Stuck = null;

        #endregion

        #region Parameters

        /// <summary>
        /// The time being passed since holding the trigger;
        /// used for judging whether the screw driver should play the END sound 
        /// </summary>
        private float m_timeBeingPassed = 0f;

        /// <summary>
        /// check whether the user are holding the trigger
        /// </summary>
        private bool m_IsPlaying = false;

        /// <summary>
        /// The time being passed since the current audio being played
        /// </summary>
        public float m_audioProgress = 0f;

        /// <summary>
        /// cache the current audio being played
        /// </summary>
        private SoundObject m_Sound = null;
        #endregion

        #region Property

        public SoundObject ScrewDriverSound
        {
            get
            {
                return m_Sound;
            }
        }

        public float AudioProgress
        {
            get
            {
                return m_audioProgress;
            }
        }

        #endregion

        #region Reference

        public ScrewDriverRotator Bit, Chuck;
        #endregion

      

        #region Unity Functions
        private void Start()
        {
            Instance = this;
        }

        private void Update()
        {
            if(m_IsPlaying)
            {
                m_timeBeingPassed += Time.deltaTime;
                m_audioProgress += Time.deltaTime;
            }
        }
        #endregion

        #region Feature Functions

        /// <summary>
        /// check Enable
        /// </summary>
        /// <param name="m_AudioClip"></param>
        /// <returns></returns>
        public bool IsActive(AudioClip m_AudioClip)
        {
            if (m_AudioClip == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// play the start sound
        /// </summary>
        public void StartPlay()
        {
            if (!IsActive(m_Begin)) { return; }
            if (m_Sound != null)
            {
                SoundPlayer.StopSound(m_Sound);
                //reset the audio progress
                m_audioProgress = 0f;
            }
            Bit.IsScrewTouched = false;

            m_Sound = new SoundObject(m_Begin, transform);
            SoundPlayer.PlayOneShot(m_Sound);
            m_IsPlaying = true;
        }


        /// <summary>
        /// play the stop sound
        /// </summary>
        public void StopPlay()
        {
            if (!IsActive(m_End)) { return; }
            if (m_Sound != null)
            {
                SoundPlayer.StopSound(m_Sound);
                //reset the audio progress
                m_audioProgress = 0f;
            }
              
            if(m_timeBeingPassed > 0.8f && Bit != null && Chuck != null)
            {
                m_Sound = new SoundObject(m_End, transform);
                SoundPlayer.PlayOneShot(m_Sound);
                //StartCoroutine(SlowDownRotation(0.003f, 10));
            }
            m_IsPlaying = false;
            m_timeBeingPassed = 0f;
        }

        /// <summary>
        /// play the fasten-screw sound
        /// </summary>
        public void FastenPlay()
        {
            if (!IsActive(m_Fasten)) { return; }
            if (m_Sound != null)
            {
                SoundPlayer.StopSound(m_Sound);
                //reset the audio progress
                m_audioProgress = 0f;
            }

            m_Sound = new SoundObject(m_Fasten, transform);
            SoundPlayer.PlayLoop(m_Sound);
        }

        /// <summary>
        /// play stuck sound
        /// </summary>
        public void StuckPlay()
        {
            if (!IsActive(m_Stuck)) { return; }
            if (m_Sound != null)
            {
                SoundPlayer.StopSound(m_Sound);
                //reset the audio progress
                m_audioProgress = 0f;
            }

            m_Sound = new SoundObject(m_Stuck, transform);
            SoundPlayer.PlayLoop(m_Sound);
        }

        #endregion

        #region Utility Functions
        /*
        private IEnumerator SlowDownRotation(float rate, int duration)
        {
            for(int i = 0; i<duration; i++)
            {
                Bit.RotateDriver();
                Chuck.RotateDriver();
                yield return new WaitForSeconds(rate * i);
            }
        }*/
        #endregion

    }
}
