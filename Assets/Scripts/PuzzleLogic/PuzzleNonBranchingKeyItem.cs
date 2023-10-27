using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleNonBranchingKeyItem : PuzzleKeyItem
{
    public bool appearsOnBothBranches;
    // Start is called before the first frame update
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
        Debug.Log("Key Item Placed:" + name);
        puzzleManager.HandleNonBranchingKeyItemPlaced();
    }
    
    // If this branching object is removed, disable Object Distance of this branch's key items
    public override void HandleKeyItemRemoved()
    {
        // If object already not in place, do nothing
        if (!objInPlace) return;
        outline.OutlineWidth = 0f;
        
        // If object was just removed from correct location
        objInPlace = false;
        Debug.Log("Key Item Removed:" + name);
        puzzleManager.HandleNonBranchingKeyItemRemoved();
    }

    // If this key item appears on both Branch A and B (ex. the guitar)
    // then provide functionality to switch between branch A's and B's puzzle targets
    public void SwitchPuzzleTargetIfItemOnBothBranches(Branch branch)
    {
        if (appearsOnBothBranches)
        {
            branch = branch;
            
        }
    }

}
