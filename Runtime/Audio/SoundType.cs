using _S.Attributes;
using System;
using UnityEngine;

namespace _S.Audio
{
    [Serializable]
    public class SoundType
    {
        public string Name;
        [SerializeField, EnumBinding(new string[] { "_sound", "_library" })]
        SoundTypes _type;
        public SoundTypes type
        {
            get { return _type; }
            set { _type = value; }
        }
        public enum SoundTypes
        {
            Sound,
            Library
        }

        [SerializeField, HideInInspector] public Sound _sound;
        public Sound sound
        {
            get { return _sound; }
            set { _sound = value; }
        }
        [SerializeField, HideInInspector] SoundLibrary _library;
        public SoundLibrary library
        {
            get { return _library; }
            set { _library = value; }
        }

        public AudioSource Source
        {
            get
            {
                switch (type)
                {
                    case SoundTypes.Sound:
                        return _sound.Source;
                    case SoundTypes.Library:
                        return _library.randomSource;
                    default:
                        return null;
                }
            }
        }

        public Sound currentSound
        {
            get
            {
                switch (type)
                {
                    case SoundTypes.Sound:
                        return _sound;
                    case SoundTypes.Library:
                        return _library.sounds[_library.index];
                    default:
                        return null;
                }
            }
        }

        public Sound getSound
        {
            get
            {
                switch (type)
                {
                    case SoundTypes.Sound:
                        return _sound;
                    case SoundTypes.Library:
                        return _library.randomSound;
                    default:
                        return null;
                }
            }
        }
    }
}