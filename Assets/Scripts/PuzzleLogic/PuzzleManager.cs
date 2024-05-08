using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static Branch currentBranch;
    private int countKeyItemsLeft;
    private VideoControls _videoControls;

    public GameObject currentBranchingItemModel;
    public GameObject memorySceneCanvas;
    [SerializeField]  public GameObject fadeBlack;
    public Animator tape2Box;

    private PlacementAudio placementAudio;

    [SerializeField] private List<GameObject> tapeObjs; // tape objects, index 0 is level 1 tape
    
    void Start()
    {
        GameState.level = 0;
        GameState.ResetRoute();
        currentBranch = Branch.None;

        _videoControls = FindObjectOfType<VideoControls>();
        placementAudio = FindObjectOfType<PlacementAudio>();

        StartNextLevel();

        PuzzleNonBranchingKeyItem.OnKeyItemPlaced += HandleNonBranchingKeyItemPlaced;
        PuzzleBranchingKeyItem.OnBranchingKeyItemPlaced += HandleBranchingItemPlaced;
    }

    private void OnDestroy()
    {
        PuzzleNonBranchingKeyItem.OnKeyItemPlaced -= HandleNonBranchingKeyItemPlaced;
        PuzzleBranchingKeyItem.OnBranchingKeyItemPlaced -= HandleBranchingItemPlaced;
        VideoControls.clipWatched -= OnClipWatched;
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

        InputManager.instance.playerInputActions.Memory.ExitMemoryScene.Disable();
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
            InputManager.instance.playerInputActions.Memory.ExitMemoryScene.Disable();
            memorySceneCanvas.SetActive(true);
            StartCoroutine(waiter());

            if (GameState.level < 2) // Start next level if any remaining
            {
                StartNextLevel(); 
            }
            else if (GameState.level == 2) // Player has completed game
            {
                VideoControls.clipWatched += OnClipWatched;
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
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(3);
        memorySceneCanvas.SetActive(false);
        InputManager.instance.ExitMemoryScene(new InputAction.CallbackContext());

        if (GameState.level == 2 && countKeyItemsLeft == 3 && currentBranch == Branch.None)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Opening tape box");
            tape2Box.SetBool("IsOpen", true);

            GameObject model = tape2Box.transform.GetChild(0).gameObject;
            InteractableDetector interactableDet = FindAnyObjectByType<InteractableDetector>();
            interactableDet.highlightObject(model);
        }
    }

    public void DisableBoxHighlight()
    {
        GameObject model = tape2Box.transform.GetChild(0).gameObject;
        InteractableDetector interactableDet = FindAnyObjectByType<InteractableDetector>();
        interactableDet.unhighlightObject(model);
    }

    void OnClipWatched()
    {
        VideoControls.clipWatched -= OnClipWatched;
        StartCoroutine(OnGameComplete());
    }

    IEnumerator OnGameComplete()
    {
        // wait for player to exit tv mode
        while (InputManager.instance.InTVMode()) { yield return null;}
        AudioController.ToggleMuteBGM();
        AudioController.instance.GetComponent<AudioSource>().Stop();
        AudioController.instance.GetComponent<AudioSource>().mute = false;

        Debug.Log("Game complete");
        if (GameState.ending == Ending.EndingB)
        {
            PlayableDirector director = GetComponent<PlayableDirector>();
            director.Play();

            while (director.time < director.duration) { yield return null; }
            
        } else
        {
            fadeBlack.SetActive(true);
            // wait for audiodiaglogue to finish
            AudioSource audioDialogue = GameObject.Find("Player").transform.Find("AudioDialogue").GetComponent<AudioSource>();
            while (audioDialogue.isPlaying) { yield return null; }
        }

        Debug.Log("Loading ending scene");
        StartCoroutine(GameLoader.LoadYourAsyncScene("Ending"));
    }

    IEnumerator completeSFXWaiter()
    {
        //Wait for 2 seconds
        yield return new WaitForSeconds(2);
        placementAudio.tapeChangeSFX();
    }
}
