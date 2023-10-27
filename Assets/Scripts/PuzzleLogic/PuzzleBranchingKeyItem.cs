using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Branching Key Item Class. Called by ObjectDistanceNew class and decides whether to call PuzzleManagerNew
/// if the branching object has been placed or removed to/from the right location
/// </summary>
public class PuzzleBranchingKeyItem : PuzzleKeyItem
{
    // Which level this branching item is part of
    public int level;
    
    // Which branch this branching item and its non branching items are part of
    public Branch branch;

    // List of the 3 key items' MODELS on this branch. This model object should contain
    // the Object Distance and Puzzle Key Item scripts.
    [SerializeField] private List<GameObject> keyItemModels;
    
    public bool timerUntilCueStarted;
    private float timerUntilShowCue;
    private float timeLeftUntilCue;
    
    // How long to wait until non branching key item cues shows up, from when this branching item is placed
    private const float WaitTimeUntilCue = 45f; 
    
    void Start()
    {
        base.Start();
        timeLeftUntilCue = WaitTimeUntilCue;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (objInPlace && timerUntilCueStarted)
        {
            timeLeftUntilCue -= Time.deltaTime;
        }

        if (timeLeftUntilCue <= 0 && timerUntilCueStarted)
        {
            showCueOfNonBranchingKeyItems();
        }
    }
    
    // If this branching object is placed in the right location, enable Object Distance of this branch's key items
    public override void HandleKeyItemPlaced()
    {
        // If object already in place, do nothing
        if (objInPlace) return;
        
        // If object was just put into place
        objInPlace = true;
        outline.OutlineWidth = 5f;
        timeLeft = timeLengthOutline;
        
        foreach (GameObject obj in keyItemModels)
        {
            Debug.Log(obj.name);
            ObjectDistanceNew objDist = obj.GetComponent<ObjectDistanceNew>();
            objDist.enabled = true;
            
            //PuzzleNonBranchingKeyItem nonBranchingKeyItem = obj.GetComponent<PuzzleNonBranchingKeyItem>();
            if (objDist.isOnBothBranches)
            {
                objDist.SwitchPuzzleTarget(branch);
            }
        }
        puzzleManager.HandleBranchingItemPlaced(branch);
        
        // start timer countdown until cue should be shown
        timerUntilCueStarted = true;
    }
    
    // If this branching object is removed, disable Object Distance of this branch's key items
    public override void HandleKeyItemRemoved()
    {
        // If object already not in place, do nothing
        if (!objInPlace) return;
        
        // If object was just removed from correct location
        objInPlace = false;
        outline.OutlineWidth = 0f;
        
        foreach (GameObject obj in keyItemModels)
        {
            // Disable Object Distance of key items of this branch
            Debug.Log(obj.name);
            ObjectDistanceNew objDist = obj.GetComponent<ObjectDistanceNew>();
            objDist.enabled = false;
            
            // Set each key item of this branch to no longer in correct location
            PuzzleNonBranchingKeyItem nonBranchingKeyItem = obj.GetComponent<PuzzleNonBranchingKeyItem>();
            nonBranchingKeyItem.HandleKeyItemRemoved();
        }
        puzzleManager.HandleBranchingItemRemoved();
        hideCueOfNonBranchingKeyItems();
        timerUntilCueStarted = false;
    }

    private void showCueOfNonBranchingKeyItems()
    {
        foreach (GameObject obj in keyItemModels)
        {
            ObjectDistanceNew objDist = obj.GetComponent<ObjectDistanceNew>();
            objDist.targetObj.SetActive(true);
            objDist.targetObj.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    
    private void hideCueOfNonBranchingKeyItems()
    {
        foreach (GameObject obj in keyItemModels)
        {
            ObjectDistanceNew objDist = obj.GetComponent<ObjectDistanceNew>();
            objDist.targetObj.transform.GetChild(0).gameObject.SetActive(false);
            objDist.targetObj.SetActive(false);
            
        }
    }
}
