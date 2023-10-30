using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManagerNew : MonoBehaviour
{
    public int level;
    public Branch branch;
    private int countKeyItemsLeft;
    private VideoControls _videoControls;
    public GameObject currentBranchingItemModel;
    
    // Start is called before the first frame update
    void Start()
    {
        _videoControls = FindObjectOfType<VideoControls>();
        level = 1;
        countKeyItemsLeft = 3;
        // TODO: Activate only level 1's branching items and non branching key items upon startup

    }

    public void HandleBranchingItemPlaced(Branch branch, GameObject placedBranchingItemModel)
    {
        currentBranchingItemModel = placedBranchingItemModel;
        if (branch == Branch.BranchA)
        {
            _videoControls.ChangeCorruptedVideo(ClipToPlay.BranchACorrupted);
        }
        else if (branch == Branch.BranchB)
        {
            _videoControls.ChangeCorruptedVideo(ClipToPlay.BranchBCorrupted);
        }
    }
    
    public void HandleNonBranchingKeyItemPlaced()
    {
        countKeyItemsLeft--;
        Debug.Log("Key items left: " + countKeyItemsLeft);
        if (countKeyItemsLeft == 0)
        {
            if (branch == Branch.BranchA)
            {
                _videoControls.CompletePuzzle(ClipToPlay.BranchASolution);
            }
            else if (branch == Branch.BranchB)
            {
                _videoControls.CompletePuzzle(ClipToPlay.BranchBSolution);
            }
            level++;
            // TODO: Deactivate current level's branching items and non branching key items
            // TODO: Activate next level's branching items and non branching key items
        }
    }

    // Call this when the player has exited the memory scene after placing a branching object in the right place
    public void ShowNonBranchingItemsShadowCues()
    {
        currentBranchingItemModel.GetComponent<PuzzleBranchingKeyItem>().ShowCuesOfNonBranchingKeyItems();
    }
}
