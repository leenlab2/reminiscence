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
    public AudioSource televisionAudioSource;
    public ParticleSystem televisionParticleEffects;
    private TapeManager _tapeManager;
    private DialogueManager _dialogueManager;

    public static Action clipWatched;

    void Start()
    {
        print(GameObject.Find("TV"));
        _tapeManager = FindObjectOfType<TapeManager>();
        _videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
        _dialogueManager = GameObject.Find("Dialogue Manager").GetComponent<DialogueManager>();

        if (_progressBarImage != null)
        {
            _progressBarImage.fillAmount = 0;
        }
        
        televisionAudioSource = GameObject.Find("TV").GetComponent<AudioSource>();
        televisionParticleEffects = GameObject.Find("TVEffectsPuzzleComplete").GetComponent<ParticleSystem>();

        InputManager.OnGamePaused += Pause;
    }

    private void OnDestroy()
    {
        InputManager.OnGamePaused -= Pause;
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
    public void CompletePuzzle(ClipToPlay clip) // TODO: Only call this method if there exists a tape in the TV. Otherwise, we wouldn't know which tape's puzzle we are currently solving
    {   
        // play confirmation noise from television
        // televisionAudioSource.Play();
        televisionParticleEffects.startColor = Color.yellow; // play particles from TV
        televisionParticleEffects.Play();
        
        // Set tape to fixed one and play from time the glitch was fixed
        TapeSO tapeSOInTV = _tapeManager.GetCurrentTapeInTV();
        tapeSOInTV.SetTapeToFixed(clip);
        tapeSOInTV.tapeSolutionBranch = clip;
        tapeSOInTV.clipToPlay = clip;
        _videoPlayer.clip = tapeSOInTV.GetVideoClip();
        
        // play for one frame to update render texture
        _videoPlayer.Play();
        _videoPlayer.Pause();
    }
    
    // Call this method to change the video when the tape is not yet completed
    public void ChangeCorruptedVideo(ClipToPlay clip)
    {
        //StartCoroutine(waiter(clip));
        TapeSO tapeSOInTV = _tapeManager.GetCurrentTapeInTV();

        televisionAudioSource.Play(); // Play noise from TV. TODO: Different noise between this and OnPuzzleComplete
        televisionParticleEffects.startColor = Color.white; // play particles from TV
        televisionParticleEffects.Play();

        if (clip == ClipToPlay.OriginalCorrupted) // switch video on TV to original corrupted
        {
            _videoPlayer.clip = tapeSOInTV.originalCorruptedVideoClip;
            tapeSOInTV.clipToPlay = ClipToPlay.OriginalCorrupted;
        }
        else if (clip == ClipToPlay.BranchACorrupted) // switch video on TV to Branch A Corrupted
        {
            _videoPlayer.clip = tapeSOInTV.branchACorruptedVideoClip;
            tapeSOInTV.clipToPlay = ClipToPlay.BranchACorrupted;
        }
        else if (clip == ClipToPlay.BranchBCorrupted) // switch video on TV to Branch B Corrupted
        {
            _videoPlayer.clip = tapeSOInTV.branchBCorruptedVideoClip;
            tapeSOInTV.clipToPlay = ClipToPlay.BranchBCorrupted;
        }
        // play for one frame to update render texture
        _videoPlayer.Play();
        _videoPlayer.Pause();
    }

    void Update()
    {
        if (_progressBarImage == null) return;

        if (_videoPlayer.length > 0)
        {
            float progressPercentage = (float) (_videoPlayer.time / _videoPlayer.length); 
            _progressBarImage.fillAmount = progressPercentage;

            //Dialogue Test
            if (progressPercentage >= 0.5f)
            {
                _dialogueManager.setDialogueText("THIS IS A TEST");
                _dialogueManager.playDialogueSubtitles();
            }

            if (progressPercentage >= 0.95f)
            {
                clipWatched?.Invoke();
            }
        }
        else
        {
            _progressBarImage.fillAmount = 0;
        }
    }
}

