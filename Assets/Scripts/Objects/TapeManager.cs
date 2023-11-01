using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TapeManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private GameObject currentTapeInTv;
    private PickUpInteractor pickUpInteractor;
    
    void Start()
    {
        videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
        pickUpInteractor = FindObjectOfType<PickUpInteractor>();
        TapeSO tapeSO = GameObject.Find("Tape").GetComponentInChildren<TapeInformation>().TapeSO;
        tapeSO.tapeIsFixed = false;
        tapeSO.clipToPlay = ClipToPlay.OriginalCorrupted;
        tapeSO.tapeSolutionBranch = ClipToPlay.OriginalCorrupted;
        
        videoPlayer.targetTexture.Release(); // ensure nothing is rendered on TV upon startup
    }

    public bool televisionHasTape()
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
            pickUpInteractor.DropHeldObject();
            
            // activate branching items of this tape
            TapeInformation tapeInfo = tapeGameObject.GetComponent<TapeInformation>();

            // TODO: Only activate branching items of this tape if PuzzleManager says we are on this tape's level
            int level = tapeInfo.TapeSO.level;
            tapeInfo.branchingItemA.GetComponent<ObjectDistance>().enabled = true;
            tapeInfo.branchingItemB.GetComponent<ObjectDistance>().enabled = true;
            ShowBranchCues();

        }
    }

    public void removeTape()
    {
        if (!televisionHasTape()) // if television does not have tape, do nothing
        {
            return;
        }
        else // if television has tape in it, remove it
        {
            // Deactivate branching items of this tape
            TapeInformation tapeInfo = currentTapeInTv.GetComponent<TapeInformation>();
            HideBranchCues();
            // TODO: Only activate branching items of this tape if PuzzleManager says we are on this tape's level
            int level = tapeInfo.TapeSO.level;
            tapeInfo.branchingItemA.GetComponent<ObjectDistance>().enabled = false;
            tapeInfo.branchingItemB.GetComponent<ObjectDistance>().enabled = false;
            
            // put obj back in hands of player and set video clip on TV to null
            // set clip on TV's player to null
            currentTapeInTv.active = true;
            videoPlayer.targetTexture.Release();
            pickUpInteractor.PickupObject(currentTapeInTv);
            videoPlayer.clip = null;
            currentTapeInTv = null;
        }
    }
    
    public TapeSO GetCurrentTapeInTV()
    {
        return currentTapeInTv.GetComponent<TapeInformation>().TapeSO;
    }

    // Show cue of branching object for whatever tape is in the TV
    private void ShowBranchCues()
    {
        TapeInformation tapeInfo = currentTapeInTv.GetComponent<TapeInformation>();
        ObjectDistance objDist = tapeInfo.branchingItemA.GetComponent<ObjectDistance>();
        objDist.targetObj.SetActive(true);
        ObjectDistance objDistB = tapeInfo.branchingItemB.GetComponent<ObjectDistance>();
        objDistB.targetObj.SetActive(true);
    }

    // Hide cue of branching object for whatever tape is in the TV
    private void HideBranchCues()
    {
        TapeInformation tapeInfo = currentTapeInTv.GetComponent<TapeInformation>();
        ObjectDistance objDist = tapeInfo.branchingItemA.GetComponent<ObjectDistance>();
        objDist.targetObj.SetActive(false);
        ObjectDistance objDistB = tapeInfo.branchingItemB.GetComponent<ObjectDistance>();
        objDistB.targetObj.SetActive(false);
    }
}