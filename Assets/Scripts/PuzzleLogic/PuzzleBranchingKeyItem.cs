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
    
    public event Action OnBranchingKeyItemPlaced;
    
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    private void OnEnable()
    {
        OnBranchingKeyItemPlaced += KeyItemPlacedPuzzleItemLogic;
    }

      public override void HandleKeyItemPlaced()
    {
        OnBranchingKeyItemPlaced?.Invoke();
    }

    // If this branching object is placed in the right location, enable Object Distance of this branch's key items
    private void KeyItemPlacedPuzzleItemLogic()
    {
        // Show outline around item
        outline.OutlineWidth = 5f;
        timeLeft = timeLengthOutline;

        // Turn off branching item's cue


        // Enable ObjectDistance scripts of three key items on this branch
        foreach (GameObject obj in keyItemModels)
        {
            Debug.Log(obj.name);
            ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
            objDist.enabled = true;
            
            if (objDist.isOnBothBranches)
            {
                objDist.SwitchPuzzleTarget(branch);
            }
        }
        
        // Disable other branching item's ObjectDistance script and Puzzle Target
        otherBranchingItem.GetComponent<ObjectDistance>().enabled = false;
        otherBranchingItem.GetComponent<PickupInteractable>().DisableWallMountable();
        otherBranchingItem.GetComponent<ObjectDistance>().targetObj.SetActive(false);
        
        puzzleManager.HandleBranchingItemPlaced(branch, gameObject);
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
