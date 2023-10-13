using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance { get; private set; }

    [SerializeField] private AudioClip[] footsteps;
    private AudioSource playerAudioSource;


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
        playerAudioSource.clip = audioClip;
        playerAudioSource.Play();
    }

    public void PlayFootsteps()
    {
        int randomIndex = Random.Range(0, footsteps.Length);
        AudioClip randomFootstep = footsteps[randomIndex];

        SwitchAndPlayAudio(randomFootstep);
    }
}
