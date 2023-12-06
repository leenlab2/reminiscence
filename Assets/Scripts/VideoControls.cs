using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Audio;

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
    
    public static Action dialoguePrompt;

    private TapeReactions tapeReactionsInTV;
    private TapeSO tapeSOInTV;
    private float timer;
    private bool hasBeenInvoked;

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
        tapeReactionsInTV = null;
        float timer = 0.5f;
        hasBeenInvoked = false;
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
        TapeReactions tapeReactionsInTV = _tapeManager.GetTapeReactionsInTV();

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

        //Get this tape's reactions if there is one
        if (_tapeManager.televisionHasTape())
        {
            tapeReactionsInTV = _tapeManager.GetTapeReactionsInTV();
            tapeSOInTV = _tapeManager.GetCurrentTapeInTV();

        }
        else
        {
            return;
        }

        if (_videoPlayer.length > 0)
        {
            float progressPercentage = (float)(_videoPlayer.time / _videoPlayer.length);
            _progressBarImage.fillAmount = progressPercentage;


            //////// DIALOGuE DIRTY/////
            if (progressPercentage >= 0)
            {
                if (tapeSOInTV.clipToPlay == ClipToPlay.OriginalCorrupted)
                {
                    _dialogueManager.SetDialogueNoObject(tapeReactionsInTV.startSubtitles, tapeReactionsInTV.start);
                    timer = 0.1f;
                }
                else if (tapeSOInTV.clipToPlay == ClipToPlay.BranchACorrupted) 
                {
                    _dialogueManager.SetDialogueNoObject(tapeReactionsInTV.middleSubtitles, tapeReactionsInTV.middle);
                    timer = 0.8f;
                }
                else if (tapeSOInTV.clipToPlay == ClipToPlay.BranchBCorrupted)
                {
                    _dialogueManager.SetDialogueNoObject(tapeReactionsInTV.middleSubtitles, tapeReactionsInTV.middle);
                    timer = 0.8f;
                }
                else if (tapeSOInTV.clipToPlay == ClipToPlay.BranchASolution)
                {
                    _dialogueManager.SetDialogueNoObject(tapeReactionsInTV.endASubtitles, tapeReactionsInTV.endA);
                    timer = 0.95f;
                }
                else if (tapeSOInTV.clipToPlay == ClipToPlay.BranchBSolution)
                {
                    _dialogueManager.SetDialogueNoObject(tapeReactionsInTV.endBSubtitles, tapeReactionsInTV.endB);
                    timer = 0.9f;
                }
            }

            if ((progressPercentage <= 0.94f) && (progressPercentage >= timer) && (!hasBeenInvoked))
            {
                dialoguePrompt?.Invoke();
                hasBeenInvoked = true;
            }

            ///////////////////////////



            if (progressPercentage >= 0.95f)
            {
                clipWatched?.Invoke();
                //hasBeenInvoked = false;
            }

            if ((progressPercentage >= 0.97f) && (hasBeenInvoked))
            {
                
                hasBeenInvoked = false;
            }


        }
        else
        {
            _progressBarImage.fillAmount = 0;
        }
    }
}

