using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance { get; private set; }

    [SerializeField] private List<AudioClip> footsteps = new List<AudioClip>();
    private AudioSource playerAudioSource;
    private float lastStepTime = 0.0f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        playerAudioSource = GameObject.Find("Player").GetComponentInChildren<AudioSource>();
    }

    public void SwitchAndPlayAudio(AudioClip audioClip)
    {
        Debug.Log("Playing audio: " + audioClip.name);
        playerAudioSource.clip = audioClip;
        playerAudioSource.Play();
    }

    public void PlayFootsteps(float velocity)
    {
        // Determine whether to play a noise depending on speed
        float timePerStep = 3 / velocity;
        if (Time.time - lastStepTime < timePerStep) { return; }

        // Set time for next step
        lastStepTime = Time.time;

        // Select a random footstep noise
        int randomIndex = Random.Range(0, footsteps.Count);
        while (footsteps[randomIndex] == playerAudioSource.clip)
        {
            randomIndex = Random.Range(0, footsteps.Count); 
        }

        AudioClip randomFootstep = footsteps[randomIndex];

        SwitchAndPlayAudio(randomFootstep);
    }
}
