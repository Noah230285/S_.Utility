using System;
using UnityEngine;
using UnityEngine.Audio;

namespace _S.Audio
{
    [DefaultExecutionOrder(-1)]
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] AudioMixerGroup _mixerGroup;
        [SerializeField] bool _3DSound;
        public SoundType[] sounds;
        string _lastName;
        //bool _instantiateSound;

        void Awake()
        {
            //if (!_instantiateSound)
            //{
            for (int i = 0; i < sounds.Length; i++)
            {
                switch (sounds[i].type)
                {
                    case SoundType.SoundTypes.Sound:
                        AddNewSoundClip(sounds[i].sound);
                        break;
                    case SoundType.SoundTypes.Library:
                        sounds[i].library = sounds[i].library.Clone();
                        for (int i2 = 0; i2 < sounds[i].library.sounds.Length; i2++)
                        {
                            AddNewSoundClip(sounds[i].library.sounds[i2]);
                        }
                        break;
                }
            }
            //}
        }


        public void PlayIsolated(RaycastHit hit)
        {
            if (hit.collider == null)
            {
                return;
            }
            string tag = hit.collider.tag;
            Play(tag);
        }

        public void PlayIsolated(string name)
        {
            var playSound = Array.Find(sounds, sound => sound.Name == name);
            if (playSound == null)
            {
                Debug.LogWarning($"Sound with name {name} not found");
                return;
            }
            _lastName = name;
            playSound.Source.PlayOneShot(playSound.Source.clip);
        }

        public void Play(RaycastHit hit)
        {
            if (hit.collider == null)
            {
                return;
            }
            string tag = hit.collider.tag;
            Play(tag);
        }

        public void Play(string name)
        {
            var playSound = Array.Find(sounds, sound => sound.Name == name);
            if (playSound.Source == null)
            {
                Debug.LogWarning($"Sound with name {name} not found");
                return;
            }
            _lastName = name;
            playSound.Source.Play();
        }

        public void Stop(RaycastHit hit)
        {
            if (hit.collider == null)
            {
                return;
            }
            string tag = hit.collider.tag;
            Stop(tag);
        }

        public void Stop(string name)
        {
            var playSound = Array.Find(sounds, sound => sound.Name == name);
            if (playSound == null)
            {
                Debug.LogWarning($"Sound with name {name} not found");
                return;
            }
            _lastName = name;
            playSound.Source.Stop();
        }

        public void MultiplyBaseVolume(RaycastHit hit, float multiplier)
        {
            if (hit.collider == null)
            {
                return;
            }
            string tag = hit.collider.tag;
            MultiplyBaseVolume(tag, multiplier);
        }

        public void MultiplyBaseVolume(string name, float multiplier)
        {
            var playSound = Array.Find(sounds, sound => sound.Name == name);
            if (playSound == null)
            {
                Debug.LogWarning($"Sound with name {name} not found");
                return;
            }
            _lastName = name;
            playSound.Source.volume = playSound.currentSound.Volume * multiplier;
        }

        void AddNewSoundClip(Sound s, GameObject gameObject = null)
        {
            s.Source = gameObject == null ? this.gameObject.AddComponent<AudioSource>() : gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;

            s.Source.outputAudioMixerGroup = _mixerGroup;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Looping;
            if (_3DSound)
            {
                s.Source.spatialBlend = 1;
            }
        }
    }

}
