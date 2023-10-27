using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TapeManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private GameObject currentTapeInTv;
    private PickUpInteractor pickUpInteractor;

    public bool timerUntilCueStarted;
    private float timerUntilShowCue;
    private float timeLeft;
    
    // How long to wait until non branching key item cues shows up, from when this branching item is placed
    private const float WaitTimeUntilCue = 3f; 
    
    void Start()
    {
        videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
        pickUpInteractor = FindObjectOfType<PickUpInteractor>();
        TapeSO tapeSO = GameObject.Find("Tape").GetComponentInChildren<TapeInformation>().TapeSO;
        tapeSO.tapeIsFixed = false;
        tapeSO.clipToPlay = ClipToPlay.OriginalCorrupted;
        tapeSO.tapeSolutionBranch = ClipToPlay.OriginalCorrupted;
        
        videoPlayer.targetTexture.Release(); // ensure nothing is rendered on TV upon startup

        timeLeft = WaitTimeUntilCue;
    }

    void Update()
    {
        HandleBranchCues();
    }

    private bool televisionHasTape()
    {
        if (currentTapeInTv != null)
        {
            return true;
        }
        return false;
    }

    public void insertTape(GameObject tapeGameObject)
    {
        if (televisionHasTape()) // if television already has tape in it, do nothing
        {
            return;
        }
        else // if television does not have tape, insert tape
        {
            // hide obj and put corresponding video clip on TV
            TapeSO tapeSO = tapeGameObject.GetComponent<TapeInformation>().TapeSO;
            videoPlayer.clip = tapeSO.GetVideoClip();
            videoPlayer.time = 0;
            
            // play first frame to update render text
            videoPlayer.Play();
            videoPlayer.Pause();
            currentTapeInTv = tapeGameObject;
            tapeGameObject.active = false;
            pickUpInteractor.DropObject();
            
            // activate branching items of this tape
            TapeInformation tapeInfo = tapeGameObject.GetComponent<TapeInformation>();

            // TODO: Only activate branching items of this tape if PuzzleManager says we are on this tape's level
            int level = tapeInfo.TapeSO.level;
            tapeInfo.branchingItemA.GetComponent<ObjectDistanceNew>().enabled = true;
            tapeInfo.branchingItemB.GetComponent<ObjectDistanceNew>().enabled = true;

            timerUntilCueStarted = true;
        }
    }

    public void removeTape()
    {
        if (!televisionHasTape()) // if television does not have tape, do nothing
        {
            print("DO NOTHING");
            return;
        }
        else // if television has tape in it, remove it
        {
            print("REMOVING");
            // Deactivate branching items of this tape
            TapeInformation tapeInfo = currentTapeInTv.GetComponent<TapeInformation>();
            // TODO: Only activate branching items of this tape if PuzzleManager says we are on this tape's level
            int level = tapeInfo.TapeSO.level;
            tapeInfo.branchingItemA.GetComponent<ObjectDistanceNew>().enabled = false;
            tapeInfo.branchingItemB.GetComponent<ObjectDistanceNew>().enabled = false;
            
            // put obj back in hands of player and set video clip on TV to null
            // set clip on TV's player to null
            currentTapeInTv.active = true;
            videoPlayer.targetTexture.Release();
            pickUpInteractor.PickupObject(currentTapeInTv);
            videoPlayer.clip = null;
            currentTapeInTv = null;
            
            timerUntilCueStarted = false;
            timeLeft = WaitTimeUntilCue;
            ObjectDistanceNew objDist = tapeInfo.branchingItemA.GetComponent<ObjectDistanceNew>();
            objDist.targetObj.SetActive(false);
        }
    }
    
    public TapeSO GetCurrentTapeInTV()
    {
        return currentTapeInTv.GetComponent<TapeInformation>().TapeSO;
    }

    private void HandleBranchCues()
    {
        if (televisionHasTape() && timerUntilCueStarted)
        {
            timeLeft -= Time.deltaTime;
        }

        if (timeLeft <= 0 && timerUntilCueStarted)
        {
            // Show cue of branching object
            TapeInformation tapeInfo = currentTapeInTv.GetComponent<TapeInformation>();
            ObjectDistanceNew objDist = tapeInfo.branchingItemA.GetComponent<ObjectDistanceNew>();
            objDist.targetObj.SetActive(true);
        }
    }
}