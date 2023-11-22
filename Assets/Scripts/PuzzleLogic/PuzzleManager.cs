using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static int level = 0;
    public static Branch currentBranch;

    private int countKeyItemsLeft;
    private VideoControls _videoControls;

    public GameObject currentBranchingItemModel;
    public GameObject memorySceneCanvas;

    private InputManager inputManager;

    [SerializeField] private List<GameObject> tapeObjs; // tape objects, index 0 is level 1 tape
    
    void Start()
    {
        _videoControls = FindObjectOfType<VideoControls>();
        inputManager = FindObjectOfType<InputManager>();

        StartNextLevel();

        PuzzleNonBranchingKeyItem.OnKeyItemPlaced += HandleNonBranchingKeyItemPlaced;
        PuzzleBranchingKeyItem.OnBranchingKeyItemPlaced += HandleBranchingItemPlaced;
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

        StartCoroutine(waiter());

    }
    
    public void HandleNonBranchingKeyItemPlaced(GameObject obj)
    {
        countKeyItemsLeft--;
        Debug.Log("Key items left: " + countKeyItemsLeft);

        if (countKeyItemsLeft == 0)
        {
            // Play puzzle complete SFX
            AudioSource completeAudio = GetComponent<AudioSource>();
            if (completeAudio != null)
            {
                completeAudio.Play();
            }

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
        Debug.Log($"Starting level: {level}");

        TapeInformation tapeInformation = tapeObjs[level - 1].GetComponentInChildren<TapeInformation>();
        tapeInformation.TapeSO.tapeIsFixed = true;
        tapeInformation.TapeSO.clipToPlay = ClipToPlay.OriginalCorrupted;
        tapeInformation.TapeSO.tapeSolutionBranch = ClipToPlay.OriginalCorrupted;

        // Turn on branching objects for this level
        tapeInformation.branchingItemA.gameObject.SetActive(true);
        tapeInformation.branchingItemB.gameObject.SetActive(true);
        
        // Turn on ObjectDistance scripts for branching items
        tapeInformation.branchingItemA.gameObject.GetComponent<ObjectDistance>().enabled = true;
        tapeInformation.branchingItemB.gameObject.GetComponent<ObjectDistance>().enabled = true;

        // Show physical tape for next level in scene
        tapeObjs[level - 1].SetActive(true);
    
        // Reset branch to none (neither A nor B)
        currentBranch = Branch.None;
    }
    
    IEnumerator waiter()
    {
        //Wait for 4 seconds
        yield return new WaitForSeconds(4);
        memorySceneCanvas.SetActive(false);
        inputManager.ExitMemoryScene(new InputAction.CallbackContext());
    }
}
