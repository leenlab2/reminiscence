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

        PuzzleNonBranchingKeyItem.OnKeyItemPlaced += PlayBGMStem;
    }

    private void OnDestroy()
    {
        PuzzleNonBranchingKeyItem.OnKeyItemPlaced -= PlayBGMStem;
    }

    public void SwitchAndPlayAudio(AudioClip audioClip)
    {
        //Debug.Log("Playing audio: " + audioClip.name);
        playerAudioSource.clip = audioClip;
        playerAudioSource.Play();
    }

    public void PlayFootsteps(float velocity)
    {
        // Determine whether to play a noise depending on speed
        float timePerStep = 5 / velocity;
        if (Time.time - lastStepTime < timePerStep) { return; }

        // Set time for next step
        lastStepTime = Time.time;

        // Determine whether to play the floor creak noise
        int randomIndex;
        float rand = Random.value;
        if (rand < 0.1f)
        {
            randomIndex = footsteps.Count - 1;
            // NOTE: this requires the floor creak noise to be the last in the list
        } else
        {
            // Select a random footstep noise
            randomIndex = Random.Range(0, footsteps.Count - 1);
            while (footsteps[randomIndex] == playerAudioSource.clip)
            {
                randomIndex = Random.Range(0, footsteps.Count - 1);
            }
        }

        AudioClip randomFootstep = footsteps[randomIndex];

        SwitchAndPlayAudio(randomFootstep);
    }

    public void PlayBGMStem(GameObject obj)
    {
        AudioSource bgm_stem = obj.GetComponentInChildren<AudioSource>();

        if (bgm_stem != null)
        {
            bgm_stem.mute = false;
        }
    }
}
