using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles when a Non Branching Key Item is placed in the right location.
/// </summary>
public class PuzzleNonBranchingKeyItem : PuzzleKeyItem
{
    public bool appearsOnBothBranches;
    
    // Note: this takes in parent root object
    public static event Action<GameObject> OnKeyItemPlaced;
    
    public override void HandleCorrectPosition()
    {
        base.HandleCorrectPosition();

        Debug.Log("Key Item Placed:" + name);

        OnKeyItemPlaced?.Invoke(transform.parent.gameObject);

        CorrectPuzzleLogic();
    }

    protected override void CorrectPuzzleLogic()
    {
        outline.OutlineWidth = 5f;
        timeLeft = timeLengthOutline;
    }
}
