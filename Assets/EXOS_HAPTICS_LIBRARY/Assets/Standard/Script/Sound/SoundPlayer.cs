using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Audio;

namespace exiii.Unity
{
    public class SoundPlayer : StaticAccessableMonoBehaviour<SoundPlayer>
    {
        [SerializeField]
        [FormerlySerializedAs("DefaultMixer")]
        private AudioMixer m_DefaultMixer = null;

        [SerializeField]
        [FormerlySerializedAs("DefaultMixerGroupSE")]
        private AudioMixerGroup m_DefaultMixerGroupSE = null;

        [SerializeField]
        [FormerlySerializedAs("MasterVolumeParamName")]
        private string m_MasterVolumeParamName = string.Empty;

        // Play one shot.
        public static SoundObject PlayOneShot(SoundObject sound)
        {
            if (!IsExist) { return null; }

            sound.outputGroup = (sound.outputGroup == null) ? Instance.m_DefaultMixerGroupSE : sound.outputGroup;
            sound.isLoop = false;

            return Instance.PlayInternal(sound);
        }

        // Play loop.
        public static SoundObject PlayLoop(SoundObject sound)
        {
            if (!IsExist) { return null; }

            sound.outputGroup = (sound.outputGroup == null) ? Instance.m_DefaultMixerGroupSE : sound.outputGroup;
            sound.isLoop = true;

            return Instance.PlayInternal(sound);
        }

        // Stop sound.
        public static void StopSound(SoundObject sound)
        {
            if (!IsExist) { return; }

            var channel = FindChannel(sound);
            if (channel == null) { return; }

            // stop.
            Instance.StopInternal(channel);
        }

        // set default mixer attn.
        public static void SetDefaultMixerMasterAttn(float attn)
        {
            if (!IsExist) { return; }

            Instance.m_DefaultMixer.SetFloat(Instance.m_MasterVolumeParamName, attn);
        }

        // check free channel.
        private bool IsExistFreeChannel()
        {
            var channels = GetComponentsInChildren<SoundChannel>();
            if (channels == null) { return false; }

            foreach (SoundChannel channel in channels)
            {
                if (channel.AudioSource == null) { continue; }
                if (channel.AudioSource.isPlaying) { continue; }

                return true;
            }

            return false;
        }

        // find free channel.
        private SoundChannel FindFreeChannel()
        {
            var channels = GetComponentsInChildren<SoundChannel>();
            if (channels == null) { return null; }

            foreach (SoundChannel channel in channels)
            {
                if (channel.AudioSource == null) { continue; }
                if (channel.AudioSource.isPlaying) { continue; }

                return channel;
            }

            return null;
        }

        // find channel by sound.
        public static SoundChannel FindChannel(SoundObject sound)
        {
            var channels = Instance.GetComponentsInChildren<SoundChannel>();
            if (channels == null) { return null; }

            foreach(SoundChannel channel in channels)
            {
                if (channel.SoundObject != sound) { continue; }

                return channel;
            }

            return null;
        }

        // play one shot.
        private SoundObject PlayInternal(SoundObject sound)
        {
            if (!IsExistFreeChannel())
            {
                EHLDebug.LogWarning("no free sound channel.", this);
                return null;
            }

            // find sound channel script.
            SoundChannel channel = FindFreeChannel();
            if (channel == null) { return null; }

            // apply infomation to channel.
            channel.ApplySoundObject(sound);

            // play.
            channel.AudioSource.Play();

            return sound;
        }

        // stop.
        private void StopInternal(SoundChannel channel)
        {
            // stop.
            channel.AudioSource.Stop();

            // release.
            channel.ReleaseSoundObject();
        }
    }
}
