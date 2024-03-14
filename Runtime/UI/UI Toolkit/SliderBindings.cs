using _S.Attributes;
using _S.ScriptableVariables;
using System;
using UnityEngine.Audio;
using UnityEngine;

namespace _S.UIToolkit
{
    [Serializable]
    public struct SliderBindings
    {
        public string Name;
        public enum SliderBindType
        {
            ClampedFloat,
            AudioMixerVolume
        }

        [Serializable]
        public struct AudioMixerValues
        {
            public AudioMixer Mixer;
            public string ValueName;
        }

        [EnumBinding(new string[] { "BindClampedFloat", "BindAudioMixer" })]
        public SliderBindType BindType;
        [HideInInspector] public ClampedFloatReference BindClampedFloat;
        [HideInInspector] public AudioMixerValues BindAudioMixer;
    }
}