using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static int level;
    public static Branch currentBranch;

    private int countKeyItemsLeft;
    private VideoControls _videoControls;

    public GameObject currentBranchingItemModel;
    public GameObject memorySceneCanvas;

    private InputManager inputManager;

    [SerializeField] private List<GameObject> tapeObjs; // tape objects, index 0 is level 1 tape

    public static Action<int> OnLevelChange;

    private int routeTracker; // 0 neutral, +ve branch A, -ve branch B
    
    void Start()
    {
        level = 0;
        routeTracker = 0;
        currentBranch = Branch.None;

        _videoControls = FindObjectOfType<VideoControls>();
        inputManager = FindObjectOfType<InputManager>();

        StartNextLevel();

        PuzzleNonBranchingKeyItem.OnKeyItemPlaced += HandleNonBranchingKeyItemPlaced;
        PuzzleBranchingKeyItem.OnBranchingKeyItemPlaced += HandleBranchingItemPlaced;
    }

    private void OnDestroy()
    {
        PuzzleNonBranchingKeyItem.OnKeyItemPlaced -= HandleNonBranchingKeyItemPlaced;
        PuzzleBranchingKeyItem.OnBranchingKeyItemPlaced -= HandleBranchingItemPlaced;
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
            if (currentBranch == Branch.BranchA)
            {
                routeTracker += 1;
                _videoControls.CompletePuzzle(ClipToPlay.BranchASolution);
            }
            else if (currentBranch == Branch.BranchB)
            {
                routeTracker -= 1;
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
                OnGameComplete();
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

        // Show physical tape for next level in scene
        tapeObjs[level - 1].SetActive(true);
    
        // Reset branch to none (neither A nor B)
        currentBranch = Branch.None;

        StartCoroutine(TurnOffAudio());
    }
    
    IEnumerator TurnOffAudio()
    {
        yield return new WaitForSeconds(5);
        OnLevelChange?.Invoke(level);
    }

    IEnumerator waiter()
    {
        //Wait for 4 seconds
        yield return new WaitForSeconds(4);
        memorySceneCanvas.SetActive(false);
        inputManager.ExitMemoryScene(new InputAction.CallbackContext());
    }

    void OnGameComplete()
    {
        Debug.Log("Game complete");
        
        if (routeTracker > 0)
        {
            Debug.Log("Music Route");
            StartCoroutine(LoadYourAsyncScene("Ending A"));
        }
        else if (routeTracker < 0)
        {
            Debug.Log("Brothers Route");
            StartCoroutine(LoadYourAsyncScene("Ending B"));
        }
        else
        {
            Debug.Log("Neutral Route");
            // TODO swap this for neutral ending
            StartCoroutine(LoadYourAsyncScene("Ending A"));
        }
    }

    // TODO: maybe merge this with gameloader later
    IEnumerator LoadYourAsyncScene(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

}
