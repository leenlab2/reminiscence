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
    BranchASolution,
    BranchBSolution
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

    // Pass in one of the solution clip ENUMs
    public void CompletePuzzle(/*ClipToPlay clip*/) // TODO: Only call this method if there exists a tape in the TV. Otherwise, we wouldn't know which tape's puzzle we are currently solving
    {   
        // play confirmation noise from television
        _televisionAudioSource.Play();
        _televisionEffectsOnPuzzleComplete.startColor = Color.yellow; // play particles from TV
        _televisionEffectsOnPuzzleComplete.Play();
        
        // Set tape to fixed one and play from time the glitch was fixed
        TapeSO tapeSOInTV = _tapeManager.GetCurrentTapeInTV();
        tapeSOInTV.SetTapeToFixed(ClipToPlay.BranchASolution); // TODO: Pass clip parameter into here instead
        _videoPlayer.clip = tapeSOInTV.GetVideoClip();
        _videoPlayer.time = tapeSOInTV.GetTimeGlitchFixedInFixedTape();
    }
    
    // Call this method to change the video when the tape is not yet completed
    public void ChangeCorruptedVideo(ClipToPlay clip)
    {
        TapeSO tapeSOInTV = _tapeManager.GetCurrentTapeInTV();
        
        _televisionAudioSource.Play(); // Play noise from TV. TODO: Different noise between this and OnPuzzleComplete
        _televisionEffectsOnPuzzleComplete.startColor = Color.white; // play particles from TV
        _televisionEffectsOnPuzzleComplete.Play();
        
        if (clip == ClipToPlay.OriginalCorrupted) // switch video on TV to original corrupted
        {
            _videoPlayer.clip = tapeSOInTV.originalCorruptedVideoClip;
        }
        else if (clip == ClipToPlay.BranchACorrupted) // switch video on TV to Branch A Corrupted
        {
            _videoPlayer.clip = tapeSOInTV.branchACorruptedVideoClip;
        }
        else if (clip == ClipToPlay.BranchBCorrupted) // switch video on TV to Branch B Corrupted
        {
            _videoPlayer.clip = tapeSOInTV.branchBCorruptedVideoClip;
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
