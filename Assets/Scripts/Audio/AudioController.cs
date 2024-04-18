using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance { get; private set; }

    [SerializeField] private List<AudioClip> footsteps = new List<AudioClip>();
    [SerializeField] private List<AudioClip> bgmStems = new List<AudioClip>();
    [SerializeField] private List<AudioSource> bgmSources = new List<AudioSource>();
    private AudioSource playerAudioSource;
    private float lastStepTime = 0.0f;
    private static float BGMDefaultVol;
    private bool[] previousMuteStates;

    public static event Action<int> OnLevelChange;

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

        BGMDefaultVol = GetComponentInChildren<AudioSource>().volume;
        previousMuteStates = new bool[bgmSources.Count];

        // Store the initial mute states of all other audio sources
        for (int i = 0; i < bgmSources.Count; i++)
        {
            previousMuteStates[i] = bgmSources[i].mute;
        }
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

    public void SwitchBase(int level)
    {
        // wait for new tape to be inserted into tv
        GetComponentInChildren<AudioSource>().clip = bgmStems[level - 1];
        GetComponentInChildren<AudioSource>().Play();

        if (level == 2)
        {
            GetComponentInChildren<AudioSource>().volume = 1f;
        }

        OnLevelChange?.Invoke(level);
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
        float rand = UnityEngine.Random.value;
        if (rand < 0.1f)
        {
            randomIndex = footsteps.Count - 1;
            // NOTE: this requires the floor creak noise to be the last in the list
        } else
        {
            // Select a random footstep noise
            randomIndex = UnityEngine.Random.Range(0, footsteps.Count - 1);
            while (footsteps[randomIndex] == playerAudioSource.clip)
            {
                randomIndex = UnityEngine.Random.Range(0, footsteps.Count - 1);
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

    public static void ChangeBGMVolume(float volume)
    {
        print("Changing BGM volume to: " + volume);
        foreach (AudioSource bgm in instance.bgmSources)
        {
            bgm.volume = (volume / 10) * BGMDefaultVol;
        }
    }

    public static void ToggleMuteBGM()
    {
        if (!instance.GetComponentInChildren<AudioSource>().mute)
        {
            //update previous mute states
            for (int i = 0; i < instance.bgmSources.Count; i++)
            {
                instance.previousMuteStates[i] = instance.bgmSources[i].mute;
                instance.bgmSources[i].mute = true;
            }
        } else
        {
            for (int i = 0; i < instance.bgmSources.Count; i++)
            {
                instance.bgmSources[i].mute = instance.previousMuteStates[i];
            }
        }
    }

    public static void ActivateFilters(bool on = true)
    {
        // for each bgm source, enable echofilter and distortion
        foreach (AudioSource bgm in instance.bgmSources)
        {
            bgm.GetComponent<AudioDistortionFilter>().enabled = on;
            bgm.GetComponent<AudioEchoFilter>().enabled = on;
        }
    }
}
