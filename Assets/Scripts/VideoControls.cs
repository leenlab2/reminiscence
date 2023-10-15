using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoControls : MonoBehaviour
{
    private VideoPlayer _videoPlayer;
    public Image _progressBarImage;
    public GameObject _televisionCanvas;
    private AudioSource _televisionAudioSource;
    private ParticleSystem _televisionEffectsOnPuzzleComplete;
    private InsertAndRemoveTape _insertAndRemoveTape;


    void Start()
    {
        _insertAndRemoveTape = FindObjectOfType<InsertAndRemoveTape>();
        _videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
        _progressBarImage.fillAmount = 0;
        _televisionAudioSource = GameObject.Find("TV").GetComponent<AudioSource>();
        _televisionEffectsOnPuzzleComplete = GameObject.Find("TVEffectsPuzzleComplete").GetComponent<ParticleSystem>();
    }

    public void PauseOrPlay()
    {
        if (_videoPlayer.isPlaying)
        {
            Pause();
        }
        else
        {
            print("Play!");
            _videoPlayer.Play();
        }
    }

    public void Pause()
    {
        print("Pause!");
        _videoPlayer.Pause();
    }
    
    public void Rewind()
    {
        print("Rewind!");
        _videoPlayer.time = _videoPlayer.time - 7;
    }

    public void FastForward()
    {
        print("FastForward!");
        _videoPlayer.time = _videoPlayer.time + 7;
    }
    
    public void Replay()
    {
        print("Replay!");
        _videoPlayer.time = 0;
        _videoPlayer.Play();
    }

    public void CompletePuzzle() // TODO: Only call this method if there exits a tape in the TV. Otherwise, we wouldn't know which tape's puzzle we are currently solving
    {
        // play confirmation noise from television
        print(_televisionAudioSource);
        _televisionAudioSource.Play();
        _televisionEffectsOnPuzzleComplete.Play();
        
        // Set tape to fixed one and play from time the glitch was fixed
        TapeSO tapeSOInTV = _insertAndRemoveTape.GetCurrentTapeInTV();
        tapeSOInTV.SetTapeToFixed();
        _videoPlayer.clip = tapeSOInTV.GetVideoClip();
        _videoPlayer.time = tapeSOInTV.GetTimeGlitchFixedInFixedTape();
    }

    void Update()
    {
        float progressPercentage = (float) (_videoPlayer.time / _videoPlayer.length); 
        _progressBarImage.fillAmount = progressPercentage;
    }
}
