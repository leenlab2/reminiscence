using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AudioClip", fileName = "New Audio Clip")]

public class AudioClipScriptableObject : ScriptableObject
{
    public AudioClip clip;

    public void SetAudioClip(AudioClip audio)
    {
        clip = audio;
    }

    public AudioClip GetSoundClip()
    {
        return clip;
    }

}
