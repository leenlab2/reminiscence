using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles when a Non Branching Key Item is placed in the right location.
/// </summary>
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
    
    public override void HandleKeyItemPlaced()
    {
        outline.OutlineWidth = 5f;
        timeLeft = timeLengthOutline;
        Debug.Log("Key Item Placed:" + name);
        puzzleManager.HandleNonBranchingKeyItemPlaced();
    }
}
