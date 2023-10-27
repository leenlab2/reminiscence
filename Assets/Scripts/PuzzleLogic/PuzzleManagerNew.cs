using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManagerNew : MonoBehaviour
{
    public int level;
    public Branch branch;
    private int countKeyItemsLeft;
    private VideoControls _videoControls;
    
    // Start is called before the first frame update
    void Start()
    {
        _videoControls = FindObjectOfType<VideoControls>();
        level = 1;
        countKeyItemsLeft = 3;
        // TODO: Activate only level 1's branching items and non branching key items upon startup

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleBranchingItemPlaced(Branch branch)
    {
        if (branch == Branch.BranchA)
        {
            _videoControls.ChangeCorruptedVideo(ClipToPlay.BranchACorrupted);
        }
        else if (branch == Branch.BranchB)
        {
            _videoControls.ChangeCorruptedVideo(ClipToPlay.BranchBCorrupted);
        }
    }

    public void HandleBranchingItemRemoved()
    {
        _videoControls.ChangeCorruptedVideo(ClipToPlay.OriginalCorrupted);
        countKeyItemsLeft = 3;
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
    
    public void HandleNonBranchingKeyItemRemoved()
    {
        if (countKeyItemsLeft < 3)
        {
            countKeyItemsLeft++;
        }
        Debug.Log("Key items left: " + countKeyItemsLeft);
    }
}
