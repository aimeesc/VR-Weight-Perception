using System;
using UnityEngine;
using UnityEngine.Audio;

namespace exiii.Unity
{
    [Serializable]
    public class SoundObject
    {
        public AudioMixerGroup outputGroup = null;
        public AudioClip playClip = null;
        public Transform playTransform = null;
        public bool isLoop = false;

        // constructor.
        public SoundObject()
        {
        }

        // constructor.
        public SoundObject(AudioMixerGroup group, AudioClip clip)
        {
            outputGroup = group;
            playClip = clip;
        }

        // constructor.
        public SoundObject(AudioMixerGroup group, AudioClip clip, Transform trans)
        {
            outputGroup = group;
            playClip = clip;
            playTransform = trans;
        }

        // constructor.
        public SoundObject(AudioClip clip)
        {
            playClip = clip;
        }

        // constructor.
        public SoundObject(AudioClip clip, Transform trans)
        {
            playClip = clip;
            playTransform = trans;
        }
    }
}
