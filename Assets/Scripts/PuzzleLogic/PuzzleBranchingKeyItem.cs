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
    
    // List of the 3 key items' ObjectDistance scripts to enable/disable
    private List<ObjectDistanceNew> keyItemsObjectDistanceScripts;
    
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
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
    }
}
