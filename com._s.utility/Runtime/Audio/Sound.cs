using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#endif
using UnityEngine;

[Serializable]
public class Sound
{
    public AudioClip Clip;

    [Range(0f, 1f)]
    public float Volume = 0.5f;

    [Range(-3f, 3f)]
    public float Pitch = 1;

    public bool Looping = false;

    //[Range(0f, 1f)]
    //public float RandomWeight = 1;

    public AudioSource Source;

    public Sound Clone()
    {
        Sound clone = new();

        clone.Clip = Clip;
        clone.Volume = Volume;
        clone.Pitch = Pitch;
        clone.Looping = Looping;
        return clone;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(Sound))]
public class SoundDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new();
        root.Add(new PropertyField(property.FindPropertyRelative("Clip")));
        root.Add(new PropertyField(property.FindPropertyRelative("Volume")));
        root.Add(new PropertyField(property.FindPropertyRelative("Pitch")));
        root.Add(new PropertyField(property.FindPropertyRelative("Looping")));

        return root;
    }
}
#endif