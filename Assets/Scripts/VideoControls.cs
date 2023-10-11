using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoControls : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public Image _progressBarImage;
    public GameObject _televisionCanvas;
    public VideoClip fixedTape;
    private AudioSource _televisionAudioSource;
    private ParticleSystem _televisionEffectsOnPuzzleComplete;


    void Start()
    {
        videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
        _progressBarImage.fillAmount = 0;
        _televisionAudioSource = GameObject.Find("TV").GetComponent<AudioSource>();
        _televisionEffectsOnPuzzleComplete = GameObject.Find("TVEffectsPuzzleComplete").GetComponent<ParticleSystem>();
    }

    public void PauseOrPlay()
    {
        if (videoPlayer.isPlaying)
        {
            Pause();
        }
        else
        {
            print("Play!");
            videoPlayer.Play();
        }
    }

    public void Pause()
    {
        print("Pause!");
        videoPlayer.Pause();
    }
    
    public void Rewind()
    {
        print("Rewind!");
        videoPlayer.time = videoPlayer.time - 7;
    }

    public void FastForward()
    {
        print("FastForward!");
        videoPlayer.time = videoPlayer.time + 7;
    }
    
    public void Replay()
    {
        print("Replay!");
        videoPlayer.time = 0;
        videoPlayer.Play();
    }

    public void CompletePuzzle()
    {
        // switch tape to fixed one
        videoPlayer.clip = fixedTape;
        videoPlayer.time = 60;
        
        // play confirmation noise from television
        print(_televisionAudioSource);
        _televisionAudioSource.Play();
        _televisionEffectsOnPuzzleComplete.Play();
    }

    void Update()
    {
        float progressPercentage = (float) (videoPlayer.time / videoPlayer.length); 
        _progressBarImage.fillAmount = progressPercentage;
    }
}
