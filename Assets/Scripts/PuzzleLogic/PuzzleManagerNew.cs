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
    [SerializeField] private List<GameObject> levelTwoBranchingObjects; // should be the MODELS of the game objects. TODO: make this extensible for multiple levels
    [SerializeField] private List<GameObject> nextLevelsTapeObjects; // tape objects, index 0 is level 1 tape
    
    void Start()
    {
        _videoControls = FindObjectOfType<VideoControls>();
        sceneManagement = FindObjectOfType<SceneManagement>();
        level = 1;
        countKeyItemsLeft = 3;
        currentBranch = Branch.None;

        PuzzleNonBranchingKeyItem.OnKeyItemPlaced += HandleNonBranchingKeyItemPlaced;
        PuzzleBranchingKeyItem.OnBranchingKeyItemPlaced += HandleBranchingItemPlaced;
        // TODO: Activate only level 1's branching items and non branching key items upon startup
    }

    public void HandleBranchingItemPlaced(GameObject placedBranchingItemModel)
    {
        currentBranchingItemModel = placedBranchingItemModel;
        currentBranch = placedBranchingItemModel.GetComponent<PuzzleBranchingKeyItem>().branch;

        if (currentBranch == Branch.BranchA)
        {
            _videoControls.ChangeCorruptedVideo(ClipToPlay.BranchACorrupted);
        }
        else if (currentBranch == Branch.BranchB)
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
    
    public void HandleNonBranchingKeyItemPlaced(GameObject obj)
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

            if (level < 2) // Start next level if any remaining
            {
                StartNextLevel(); 
            }
            else if (level == 2) // Player has completed game
            {
                // TODO: DoSomething for game complete
                Debug.Log("You have completed the game!");
            }

        }
    }

    private void StartNextLevel()
    {
        level++;
        countKeyItemsLeft = 3;
        
        // Enable Object Distance for branching items in level 2
        foreach (GameObject obj in levelTwoBranchingObjects)
        {
            obj.SetActive(true);
            obj.GetComponent<ObjectDistance>().enabled = true;
        }
        
        // Show physical tape for next level in scene
        nextLevelsTapeObjects[level - 1].SetActive(true);
    
        // Reset branch to none (neither A nor B)
        currentBranch = Branch.None;
        
        // Ensure Tape 2 is set to play from original clip
        TapeSO tapeSOTwo = GameObject.Find("Tape 2").GetComponentInChildren<TapeInformation>().TapeSO;
        tapeSOTwo.tapeIsFixed = false;
        tapeSOTwo.clipToPlay = ClipToPlay.OriginalCorrupted;
        tapeSOTwo.tapeSolutionBranch = ClipToPlay.OriginalCorrupted;
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
