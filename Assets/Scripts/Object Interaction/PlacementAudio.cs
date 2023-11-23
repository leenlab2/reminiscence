using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementAudio : MonoBehaviour
{
    [SerializeField] private AudioClip correctKey;
    [SerializeField] private AudioClip tapeChange;
    private AudioSource placementAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        placementAudioSource = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void correctKeyPlacementSFX()
    {
        //Debug.Log("Playing audio: " + audioClip.name);
        placementAudioSource.clip = correctKey;
        placementAudioSource.Play();
    }

    public void tapeChangeSFX()
    {
        //Debug.Log("Playing audio: " + audioClip.name);
        placementAudioSource.clip = tapeChange;
        placementAudioSource.Play();
    }
}
