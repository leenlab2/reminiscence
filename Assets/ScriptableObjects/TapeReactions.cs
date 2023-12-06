using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Scriptable Objects/TapeReactions", fileName = "New Tape Reaction information")]

public class TapeReactions : ScriptableObject
{
    public string tapeNumber;
    public AudioClip start;
    public AudioClip middle;

    public AudioClip endA;
    public AudioClip endB;

    public string startSubtitles;
    public string middleSubtitles;

    public string endASubtitles;
    public string endBSubtitles;

}
