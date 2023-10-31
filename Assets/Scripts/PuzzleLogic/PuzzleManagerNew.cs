using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManagerNew : MonoBehaviour
{
    public int level;
    public Branch currentBranch;
    private int countKeyItemsLeft;
    private VideoControls _videoControls;
    public GameObject currentBranchingItemModel;
    public GameObject enterMemoryButton;
    public GameObject memorySceneCanvas;
    private SceneManagement sceneManagement;
    
    void Start()
    {
        _videoControls = FindObjectOfType<VideoControls>();
        sceneManagement = FindObjectOfType<SceneManagement>();
        level = 1;
        countKeyItemsLeft = 3;
        currentBranch = Branch.None;
        
        // TODO: Activate only level 1's branching items and non branching key items upon startup

    }

    public void HandleBranchingItemPlaced(Branch branch, GameObject placedBranchingItemModel)
    {
        currentBranchingItemModel = placedBranchingItemModel;
        currentBranch = branch;
        if (branch == Branch.BranchA)
        {
            _videoControls.ChangeCorruptedVideo(ClipToPlay.BranchACorrupted);
        }
        else if (branch == Branch.BranchB)
        {
            _videoControls.ChangeCorruptedVideo(ClipToPlay.BranchBCorrupted);
        }
        memorySceneCanvas.SetActive(true);

        // if first time player placed branching item, show enter memory scene button on TV for future entrances
        if (level == 1)
        {
            sceneManagement.DisableAutomaticEnterMemoryScene();
            enterMemoryButton.SetActive(true);
        }

        StartCoroutine(waiter());

    }
    
    public void HandleNonBranchingKeyItemPlaced()
    {
        countKeyItemsLeft--;
        Debug.Log("Key items left: " + countKeyItemsLeft);
        if (countKeyItemsLeft == 0)
        {
            if (currentBranch == Branch.BranchA)
            {
                _videoControls.CompletePuzzle(ClipToPlay.BranchASolution);
            }
            else if (currentBranch == Branch.BranchB)
            {
                _videoControls.CompletePuzzle(ClipToPlay.BranchBSolution);
            }
            memorySceneCanvas.SetActive(true);
            StartCoroutine(waiter());
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
    
    IEnumerator waiter()
    {
        //Wait for 4 seconds
        yield return new WaitForSeconds(4);
        sceneManagement.ExitMemoryScene();
    }
}
