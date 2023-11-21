using System;
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
    
    // The other branching item that could have been placed instead. We need a reference
    // to otherBranchingItem to disable their ObjectDistance script
    [SerializeField] public GameObject otherBranchingItem;
    
    // Note: this takes in 'Model' child object
    public static event Action<GameObject> OnBranchingKeyItemPlaced;

    public override void HandleCorrectPosition()
    {
        base.HandleCorrectPosition();
        Debug.Log("Branchin Item Placed:" + transform.parent.name);
        OnBranchingKeyItemPlaced?.Invoke(gameObject);
        CorrectPuzzleLogic();
    }

    // If this branching object is placed in the right location, enable Object Distance of this branch's key items
    protected override void CorrectPuzzleLogic()
    {
        // Show outline around item
        outline.OutlineWidth = 5f;
        timeLeft = timeLengthOutline;


        // Enable ObjectDistance scripts of three key items on this branch
        foreach (GameObject obj in keyItemModels)
        {
            ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
            objDist.enabled = true;
            
            if (objDist.isOnBothBranches)
            {
                objDist.SwitchPuzzleTarget(branch);
            }
        }

        ShowCuesOfNonBranchingKeyItems();

        // Disable other branching item
        otherBranchingItem.transform.parent.gameObject.SetActive(false);
    }

    public void ShowCuesOfNonBranchingKeyItems()
    {
        foreach (GameObject obj in keyItemModels)
        {
            ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
            objDist.targetObj.SetActive(true);
            objDist.targetObj.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    
    public void HideCuesOfNonBranchingKeyItems()
    {
        foreach (GameObject obj in keyItemModels)
        {
            ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
            objDist.targetObj.transform.GetChild(0).gameObject.SetActive(false);
            objDist.targetObj.SetActive(false);
        }
    }
}
