using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public enum ClipToPlay
{
    OriginalCorrupted,
    BranchACorrupted,
    BranchBCorrupted, 
}

public class VideoControls : MonoBehaviour
{
    private VideoPlayer _videoPlayer;
    public Image _progressBarImage;
    public GameObject _televisionCanvas;
    private AudioSource _televisionAudioSource;
    private ParticleSystem _televisionEffectsOnPuzzleComplete;
    private TapeManager _tapeManager;

    void Start()
    {
        _tapeManager = FindObjectOfType<TapeManager>();
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

    public void CompletePuzzle() // TODO: Only call this method if there exists a tape in the TV. Otherwise, we wouldn't know which tape's puzzle we are currently solving
    {   
        print("Completed 1");
        // play confirmation noise from television
        _televisionAudioSource.Play();
        _televisionEffectsOnPuzzleComplete.startColor = Color.yellow; // play particles from TV
        _televisionEffectsOnPuzzleComplete.Play();
        print("Completed 2");
        
        // Set tape to fixed one and play from time the glitch was fixed
        TapeSO tapeSOInTV = _tapeManager.GetCurrentTapeInTV();
        tapeSOInTV.SetTapeToFixed("A"); // TODO: Choose which solution branch is reached: A or B. Hard coded A for now.
        _videoPlayer.clip = tapeSOInTV.GetVideoClip();
        _videoPlayer.time = tapeSOInTV.GetTimeGlitchFixedInFixedTape();
        print("Completed 3");
    }
    
    // Call this method to change the video when the tape is not yet completed
    public void ChangeCorruptedVideo(ClipToPlay clip)
    {
        _televisionAudioSource.Play(); // Play noise from TV. TODO: Different noise between this and OnPuzzleComplete
        _televisionEffectsOnPuzzleComplete.startColor = Color.white; // play particles from TV
        _televisionEffectsOnPuzzleComplete.Play();
        
        if (clip == ClipToPlay.OriginalCorrupted) // switch video on TV to original corrupted
        {
            
        }
        else if (clip == ClipToPlay.BranchACorrupted) // switch video on TV to Branch A Corrupted
        {
            
        }
        else if (clip == ClipToPlay.BranchBCorrupted) // switch video on TV to Branch B Corrupted
        {
            
        }
    }

    void Update()
    {
        if (_videoPlayer.length > 0)
        {
            float progressPercentage = (float) (_videoPlayer.time / _videoPlayer.length); 
            _progressBarImage.fillAmount = progressPercentage;
        }
        else
        {
            _progressBarImage.fillAmount = 0;
        }
    }
}
