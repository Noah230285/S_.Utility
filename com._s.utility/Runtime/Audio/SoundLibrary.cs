using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Random Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [SerializeField] Sound[] _sounds;
    public float Volume = 0.5f;
    [HideInInspector] public int index;

    public Sound[] sounds
    {
        get { return _sounds; }
        set { _sounds = value; }
    }

    public Sound randomSound
    {
        get
        {
            index = (int)Mathf.Floor(Random.value * _sounds.Length);
            return _sounds[index];
        }
    }

    public AudioSource randomSource
    {
        get
        {
            return randomSound.Source;
        }
    }

    public SoundLibrary Clone() 
    {
        SoundLibrary clone = Instantiate(this);
        Sound[] _cloneSounds = new Sound[_sounds.Length];
        for (int i = 0; i < _cloneSounds.Length; i++)
        {
            _cloneSounds[i] = _sounds[i].Clone();
        }
        clone.sounds = _cloneSounds;
        clone.Volume = Volume;
        return clone;
    }
}
