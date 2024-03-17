using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InputManager : MonoBehaviour
{
    public PlayerInputActions playerInputActions;
    public GameObject pauseMenu;
    public GameObject resumeButton;
    public AudioSource uiAudio;

    private Rigidbody playerBody;

    private float _mouseSensitivity = 1.8f;
    private Animator playerAnimate;
    
    private float _sprintSpeed = 9f;
    private float _walkSpeed = 7f;
    private float _speed;

    private InteractionCue _interactionCue;
    private DialogueManager _dialogueManager;
    private GameObject currSelectedBranching = null;
    private GameObject oldSelected;
    private GameObject hud;

    private bool inTVMode = false;
    private bool inMemoryMode = false;
    private bool gamePaused = false;
    private bool inBranchingSelection = false;
    private bool inspectionMode = false;

    public static Action OnGamePaused;
    public static Action OnGameResumed;

    private void Awake()
    {
        _speed = _walkSpeed;
        playerBody = GetComponent<Rigidbody>();
        playerAnimate = GetComponent<Animator>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Television.Disable();
        playerInputActions.Inspect.Disable();
        playerInputActions.Branching.Disable();
        playerInputActions.Memory.Disable();

        AssignPlayerHandlers();
        AssignTelevisionHandlers();
        AssignMemoryHandlers();
        AssignBranchingHandlers();
        AssignInspectionHandlers();

        playerInputActions.UI.Pause.performed += PauseGame;
        playerInputActions.UI.Pause.Enable();   
    }

    #region Assigning Handlers
    private void AssignPlayerHandlers()
    {
        playerInputActions.Player.OpenTV.performed += OpenTelevision;
        playerInputActions.Player.Interact.performed += ObjectInteract;
        playerInputActions.Player.InspectionToggle.performed += ObjectInspectionToggle;
        playerInputActions.Player.Move.canceled += StopPlayerMove;
    }

    private void AssignTelevisionHandlers()
    {
        playerInputActions.Television.CloseTV.performed += CloseTelevision;
    }

    private void AssignMemoryHandlers()
    {
        playerInputActions.Memory.ExitMemoryScene.performed += ExitMemoryScene;
    }

    private void AssignBranchingHandlers()
    {
        PickUpInteractor.OnBranchingPickup += BranchingItemPickedUp;
        playerInputActions.Branching.Navigate.performed += SwitchBranchingItem;
        playerInputActions.Branching.Submit.performed += SubmitBranchingItem;
    }

    private void AssignInspectionHandlers()
    {
        playerInputActions.Inspect.InspectionToggle.performed += ObjectInspectionToggle;
    }

    private void OnDestroy()
    {
        // unsubscribe from all events assigned in previous functions
        playerInputActions.Player.OpenTV.performed -= OpenTelevision;
        playerInputActions.Player.Interact.performed -= ObjectInteract;
        playerInputActions.Player.InspectionToggle.performed -= ObjectInspectionToggle;
        playerInputActions.Player.Move.canceled -= StopPlayerMove;

        playerInputActions.UI.Pause.performed -= PauseGame;

        playerInputActions.Television.CloseTV.performed -= CloseTelevision;

        playerInputActions.Memory.ExitMemoryScene.performed -= ExitMemoryScene;

        PickUpInteractor.OnBranchingPickup -= BranchingItemPickedUp;
        playerInputActions.Branching.Navigate.performed -= SwitchBranchingItem;
        playerInputActions.Branching.Submit.performed -= SubmitBranchingItem;

        playerInputActions.Inspect.InspectionToggle.performed -= ObjectInspectionToggle;
    }
    #endregion

    private void Start()
    {
        _interactionCue = GameObject.Find("InteractionCue").GetComponent<InteractionCue>();
        _dialogueManager = GameObject.Find("Dialogue Manager").GetComponent<DialogueManager>();
        inTVMode = false;

        hud = GameObject.Find("HUD");
    }

    private void FixedUpdate()
    {
        if (inTVMode || gamePaused) return;

        MovePlayer();
        MoveCamera();
        ObjectRotation();

    }


    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    #region Pause Menu
    private void PauseGame(InputAction.CallbackContext obj)
    {
        gamePaused = true;
        Time.timeScale = 0;
        uiAudio.Play();
        pauseMenu.SetActive(true);
        oldSelected = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(resumeButton);

        AudioController.ChangeBGMVolume(0);

        playerInputActions.Player.Disable();
        playerInputActions.UI.Pause.Disable(); 

        hud.SetActive(false);

        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        gamePaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false); 

        if (!inTVMode)
        {
            playerInputActions.Player.Enable();
        }
        playerInputActions.UI.Pause.Enable();

        AudioController.ChangeBGMVolume(1);

        EventSystem.current.SetSelectedGameObject(oldSelected);

        hud.SetActive(true);

        OnGameResumed?.Invoke();
    }

    public bool isPaused()
    {
        return gamePaused;
    }
    #endregion

    #region Player Movement
    private void StopPlayerMove(InputAction.CallbackContext obj)
    {
        playerBody.velocity = Vector3.zero;
    }

    private void MovePlayer()
    {
        Vector2 movementInput = playerInputActions.Player.Move.ReadValue<Vector2>();

        // Movement needs to be in local space axis not world space
        Vector3 movement = transform.TransformDirection(new Vector3(movementInput.x, 0, movementInput.y)) * _speed;
        playerBody.velocity = movement;

        if (movementInput.sqrMagnitude > 0)
        {
            playerAnimate.SetBool("Walking", true);
        }
        else
        {
            playerAnimate.SetBool("Walking", false);
        }

        // Setup audio playback based on movement input
        if (playerInputActions.Player.Move.phase == InputActionPhase.Started)
        {
            AudioController.instance.PlayFootsteps(playerBody.velocity.magnitude);
            Invoke("SpeedUp", 1.5f);

        }

        if (playerInputActions.Player.Move.phase != InputActionPhase.Started)
        {
            _speed = _walkSpeed;
        }
    }

    private void SpeedUp()
    {
        _speed = _sprintSpeed;
    }

    private void MoveCamera()
    {
        Vector2 cameraInput;
        cameraInput = playerInputActions.Player.Look.ReadValue<Vector2>();

        // Move the player to look around left/right when mouse pans left/right
        transform.Rotate(0, cameraInput.x * (float) Math.Tan(_mouseSensitivity * 0.5), 0);

        // Move the camera to look around up/down when mouse pans up/down
        Transform playerCamera = GetComponentInChildren<Camera>().transform;

        
        float x_rotation = Mathf.Clamp(playerCamera.localRotation.eulerAngles.x - cameraInput.y * _mouseSensitivity, -60f, 360f);
        playerCamera.localRotation = Quaternion.Euler(x_rotation, playerCamera.localRotation.y, playerCamera.localRotation.z);

        //playerCamera.Rotate(-cameraInput.y * _mouseSensitivity, 0, 0);
    }

    public void changeCameraSpeed(float sliderVal)
    {
        _mouseSensitivity = 1.8f * sliderVal;
    }
    #endregion

    #region Television Toggle
    private void OpenTelevision(InputAction.CallbackContext obj)
    {
        Debug.Log("Opening TV");
        playerInputActions.Player.Disable();
        playerInputActions.Television.Enable();

        inTVMode = true;

        ChangeCameraPosition cameraCtrl = GetComponentInChildren<ChangeCameraPosition>();
        cameraCtrl.SwitchToTapeView();
    }

    public void CloseTelevision(InputAction.CallbackContext obj)
    {
        Debug.Log("Close TV");
        inTVMode = false;

        playerInputActions.Television.Disable();
        playerInputActions.Player.Enable();

        ChangeCameraPosition cameraCtrl = GetComponentInChildren<ChangeCameraPosition>();
        cameraCtrl.SwitchToPlayerView();
    }

    public bool InTVMode()
    {
        return inTVMode;
    }

    #endregion

    #region SceneManagement
    public void EnterMemoryScene()
    {
        CloseTelevision(new InputAction.CallbackContext());
        playerInputActions.Player.OpenTV.Disable();
        playerInputActions.Memory.Enable();
        inMemoryMode = true;
    }

    public void ExitMemoryScene(InputAction.CallbackContext obj)
    {
        playerInputActions.Memory.Disable();
        playerInputActions.Player.OpenTV.Enable();
        SceneManagement sceneManagement = FindObjectOfType<SceneManagement>();
        sceneManagement.ExitMemoryScene();
        inMemoryMode = false;
    }

    public bool isInMemoryMode()
    {
        return inMemoryMode;
    }


    #endregion

    #region Object Interactions
    private void ObjectInteract(InputAction.CallbackContext context)
    {
        if (context.interaction is not HoldInteraction)
        {            
            InteractableDetector interactableDetector = GetComponent<InteractableDetector>();
            interactableDetector.InteractWithObject();
        }
    }

    #region Object Inspection
    private void ObjectRotation()
    {
        Vector2 rotationInput = playerInputActions.Inspect.Rotate.ReadValue<Vector2>();
        Inspection inspection = GetComponentInChildren<Inspection>();
        inspection.RotateObject(rotationInput);
    }

    private void ObjectInspectionToggle(InputAction.CallbackContext ctx)
    {
        Inspection inspection = GetComponentInChildren<Inspection>();
        if (!inspection.InspectIsValid()) return;

        inspectionMode = !inspectionMode;

        InteractionCue interactionCue = GetComponent<InteractionCue>();

        if (inspectionMode)
        {
            Debug.Log("Toggling On Inspect");
            _interactionCue.SetInteractionCue(InteractionCueType.Inspection);
            playerInputActions.Player.Disable();
            playerInputActions.Inspect.Enable();

            inspection.ToggleFocusObject(true);
            _dialogueManager.playDialogue();
        }
        else
        {
            Debug.Log("Toggling Off Inspect");
            _interactionCue.SetInteractionCue(InteractionCueType.Hold);
            playerInputActions.Player.Enable();
            playerInputActions.Inspect.Disable();
            inspection.ToggleFocusObject(false);

            //Stop dialogue
            _dialogueManager.stopDialogue();
        }
    }

    public bool InInspection()
    {
        return inspectionMode;
    }
    #endregion
    #endregion

    #region Branching Item
    void BranchingItemPickedUp(GameObject obj)
    {
        currSelectedBranching = obj;
        playerInputActions.Player.Disable();
        playerInputActions.Branching.Enable();
        inBranchingSelection = true;

        //Update dialogue and play audio
        _dialogueManager.setDialogue(currSelectedBranching);
        _dialogueManager.playBranchingDialogue();
    }

    void SwitchBranchingItem(InputAction.CallbackContext ctx)
    {
        Debug.Log("Switching branching item");
        InteractableDetector interactableDetect = GetComponent<InteractableDetector>();
        interactableDetect.unhighlightObject(currSelectedBranching);
        GameObject otherBranching = currSelectedBranching.GetComponent<PuzzleBranchingKeyItem>().otherBranchingItem;
        interactableDetect.highlightObject(otherBranching);

        currSelectedBranching = otherBranching;

        //Update dialogue and play audio
        _dialogueManager.stopDialogue();
        _dialogueManager.setDialogue(currSelectedBranching);
        _dialogueManager.playBranchingDialogue();

    }

    void SubmitBranchingItem(InputAction.CallbackContext ctx)
    {
        InteractableDetector interactableDetect = GetComponent<InteractableDetector>();
        interactableDetect.unhighlightObject(currSelectedBranching);

        PickUpInteractor pickupInteractor = GetComponent<PickUpInteractor>();
        pickupInteractor.SelectBranchingItem(currSelectedBranching);

        playerInputActions.Branching.Disable();
        playerInputActions.Player.Enable();

        inBranchingSelection = false;

        //Stop the dialogue on select
        _dialogueManager.stopDialogue();
    }

    public bool isInBranchingSelection()
    {
        return inBranchingSelection;
    }
    #endregion
}
