using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity.Sound
{
    public class SoundDirectEmitter : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("AudioClip")]
        private AudioClip m_AudioClip = null;

        // check enable.
        public bool IsActive()
        {
            if (m_AudioClip == null)
            {
                return false;
            }

            return true;
        }

        // Play direct.
        public void PlayOneShot()
        {
            if (!IsActive()) { return; }

            SoundObject sound = new SoundObject(m_AudioClip, transform);
            SoundPlayer.PlayOneShot(sound);
        }
    }
}
