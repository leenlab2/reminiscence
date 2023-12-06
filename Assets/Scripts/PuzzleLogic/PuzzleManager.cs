using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static Branch currentBranch;
    private int countKeyItemsLeft;
    private VideoControls _videoControls;

    public GameObject currentBranchingItemModel;
    public GameObject memorySceneCanvas;
    public Animator tape2Box; 

    private InputManager inputManager;
    private PlacementAudio placementAudio;

    [SerializeField] private List<GameObject> tapeObjs; // tape objects, index 0 is level 1 tape

    public static Action<int> OnLevelChange;
    
    void Start()
    {
        GameState.level = 0;
        GameState.ResetRoute();
        currentBranch = Branch.None;

        _videoControls = FindObjectOfType<VideoControls>();
        inputManager = FindObjectOfType<InputManager>();
        placementAudio = FindObjectOfType<PlacementAudio>();

        StartNextLevel();

        PuzzleNonBranchingKeyItem.OnKeyItemPlaced += HandleNonBranchingKeyItemPlaced;
        PuzzleBranchingKeyItem.OnBranchingKeyItemPlaced += HandleBranchingItemPlaced;
    }

    private void OnDestroy()
    {
        PuzzleNonBranchingKeyItem.OnKeyItemPlaced -= HandleNonBranchingKeyItemPlaced;
        PuzzleBranchingKeyItem.OnBranchingKeyItemPlaced -= HandleBranchingItemPlaced;
        VideoControls.clipWatched -= OnGameComplete;
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

        placementAudio.correctKeyPlacementSFX();
        
        if (countKeyItemsLeft == 0)
        {
            StartCoroutine(completeSFXWaiter());
            if (currentBranch == Branch.BranchA)
            {
                GameState.RecordRoute(true);
                _videoControls.CompletePuzzle(ClipToPlay.BranchASolution);
            }
            else if (currentBranch == Branch.BranchB)
            {
                GameState.RecordRoute(false);
                _videoControls.CompletePuzzle(ClipToPlay.BranchBSolution);
            }
            memorySceneCanvas.SetActive(true);
            StartCoroutine(waiter());

            if (GameState.level < 2) // Start next level if any remaining
            {
                StartNextLevel(); 
            }
            else if (GameState.level == 2) // Player has completed game
            {
                VideoControls.clipWatched += OnGameComplete;
            }

        }
    }

    private void StartNextLevel()
    {
        GameState.level++;
        countKeyItemsLeft = 3;
        Debug.Log($"Starting level: {GameState.level}");

        TapeInformation tapeInformation = tapeObjs[GameState.level - 1].GetComponentInChildren<TapeInformation>();
        tapeInformation.TapeSO.tapeIsFixed = true;
        tapeInformation.TapeSO.clipToPlay = ClipToPlay.OriginalCorrupted;
        tapeInformation.TapeSO.tapeSolutionBranch = ClipToPlay.OriginalCorrupted;

        // Turn on branching objects for this level
        tapeInformation.branchingItemA.gameObject.SetActive(true);
        tapeInformation.branchingItemB.gameObject.SetActive(true);

        // Show physical tape for next level in scene
        tapeObjs[GameState.level - 1].SetActive(true);
    
        // Reset branch to none (neither A nor B)
        currentBranch = Branch.None;

        StartCoroutine(TurnOffAudio());
    }
    
    IEnumerator TurnOffAudio()
    {
        yield return new WaitForSeconds(1);
        OnLevelChange?.Invoke(GameState.level);
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(3);
        memorySceneCanvas.SetActive(false);
        inputManager.ExitMemoryScene(new InputAction.CallbackContext());

        if (GameState.level == 2 && countKeyItemsLeft == 3 && currentBranch == Branch.None)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Opening tape box");
            tape2Box.SetBool("IsOpen", true);
        }
    }

    void OnGameComplete()
    {
        Debug.Log("Game complete");
        StartCoroutine(GameLoader.LoadYourAsyncScene("Ending"));
    }

    IEnumerator completeSFXWaiter()
    {
        //Wait for 2 seconds
        yield return new WaitForSeconds(2);
        placementAudio.tapeChangeSFX();
    }
}
