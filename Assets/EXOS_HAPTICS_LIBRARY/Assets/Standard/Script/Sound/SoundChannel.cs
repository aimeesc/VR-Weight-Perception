using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace exiii.Unity
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundChannel : ExMonoBehaviour
    {
        [SerializeField, Unchangeable]
        private AudioSource m_AudioSource = null;
        public AudioSource AudioSource { get { return this.m_AudioSource; } }

        [SerializeField, Unchangeable]
        [FormerlySerializedAs("SoundObject")]
        private SoundObject m_SoundObject = null;
        public SoundObject SoundObject { get { return this.m_SoundObject; } }

        // awake.
        protected override void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();

            base.Awake();
        }

        // update.
        public void Update()
        {
            // update transform.
            UpdateTransform();
        }

        // apply sound object.
        public void ApplySoundObject(SoundObject sound)
        {
            if (m_AudioSource == null) { return; }

            m_SoundObject = sound;

            // init transform.
            transform.localPosition = Vector3.zero;
            if (sound.playTransform != null)
            {
                transform.position = sound.playTransform.position;
            }

            // apply information.
            m_AudioSource.outputAudioMixerGroup = sound.outputGroup;
            m_AudioSource.clip = sound.playClip;
            m_AudioSource.loop = sound.isLoop;
        }

        // remove sound object.
        public void ReleaseSoundObject()
        {
            if (m_AudioSource == null) { return; }

            m_SoundObject = null;
        }

        // change sound pitch.
        public void ChangePitch(float pitch)
        {
            if (m_AudioSource == null) { return; }

            m_AudioSource.pitch = pitch;
        }

        // change sound volume.
        public void ChangeVolume(float volume)
        {
            if (m_AudioSource == null) { return; }

            m_AudioSource.volume = volume;
        }

        // update transform.
        private void UpdateTransform()
        {
            if (m_SoundObject == null || m_SoundObject.playTransform == null) { return; }
            this.transform.position = m_SoundObject.playTransform.position;
        }
    }
}
