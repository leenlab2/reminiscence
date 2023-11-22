using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InputManager : MonoBehaviour
{
    public PlayerInputActions playerInputActions;

    private Rigidbody playerBody;

    private float _mouseSensitivity = 3f;
    
    private float _sprintSpeed = 9f;
    private float _walkSpeed = 7f;
    private float _speed;
    private bool inspectionMode = false;

    private InteractionCue _interactionCue;
    private GameObject currSelectedBranching = null;

    private bool inTVMode = false;

    private bool inBranchingSelection;

    private void Awake()
    {
        _speed = _walkSpeed;
        playerBody = GetComponent<Rigidbody>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Television.Disable();
        playerInputActions.Inspect.Disable();
        playerInputActions.Branching.Disable();

        // Player Input Map
        playerInputActions.Player.OpenTV.performed += OpenTelevision;
        playerInputActions.Player.Interact.performed += ctx =>
        {
            if (ctx.interaction is not HoldInteraction) ObjectInteract(ctx);
        };
        playerInputActions.Player.InspectionToggle.performed += ObjectInspectionToggle;
        
        // Memory Input Map
        playerInputActions.Memory.Disable();
        playerInputActions.Memory.ExitMemoryScene.performed += ExitMemoryScene;

        // Television Input Map
        playerInputActions.Television.CloseTV.performed += CloseTelevision;

        // Inspection Input Map
        playerInputActions.Inspect.InspectionToggle.performed += ObjectInspectionToggle;

        // Branching Input Map
        PickUpInteractor.OnBranchingPickup += BranchingItemPickedUp;
        playerInputActions.Branching.Navigate.performed += SwitchBranchingItem;
        playerInputActions.Branching.Submit.performed += SubmitBranchingItem;

        inBranchingSelection = false;
    }

    private void Start()
    {
        _interactionCue = GameObject.Find("InteractionCue").GetComponent<InteractionCue>();
        inTVMode = false;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        MoveCamera();
        ObjectRotation();

    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }

    #region Player Movement
    private void MovePlayer()
    {
        Vector2 movementInput = playerInputActions.Player.Move.ReadValue<Vector2>();

        // Movement needs to be in local space axis not world space
        Vector3 movement = transform.TransformDirection(new Vector3(movementInput.x, 0, movementInput.y)) * _speed;
        playerBody.velocity = movement;

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
        if (playerInputActions.Player.enabled)
        {
            cameraInput = playerInputActions.Player.Look.ReadValue<Vector2>();
        } else
        {
            cameraInput = playerInputActions.Branching.Look.ReadValue<Vector2>();
        }

        // Move the player to look around left/right when mouse pans left/right
        transform.Rotate(0, cameraInput.x * _mouseSensitivity * 1.2f, 0);

        // Move the camera to look around up/down when mouse pans up/down
        Transform playerCamera = GetComponentInChildren<Camera>().transform;

        
        float x_rotation = Mathf.Clamp(playerCamera.localRotation.eulerAngles.x - cameraInput.y * _mouseSensitivity, -60f, 360f);
        playerCamera.localRotation = Quaternion.Euler(x_rotation, playerCamera.localRotation.y, playerCamera.localRotation.z);

        //playerCamera.Rotate(-cameraInput.y * _mouseSensitivity, 0, 0);
    }
    #endregion

    #region Television Toggle
    private void OpenTelevision(InputAction.CallbackContext obj)
    {
        Debug.Log("Opening TV button pressed");
        playerInputActions.Player.Disable();
        playerInputActions.Television.Enable();

        inTVMode = true;

        ChangeCameraPosition cameraCtrl = GetComponentInChildren<ChangeCameraPosition>();
        cameraCtrl.SwitchToTapeView();
        
        SceneManagement sceneManagement = FindObjectOfType<SceneManagement>();
    }

    public void CloseTelevision(InputAction.CallbackContext obj)
    {
        playerInputActions.Television.Disable();
        playerInputActions.Player.Enable();

        inTVMode = false;

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
    }

    public void ExitMemoryScene(InputAction.CallbackContext obj)
    {
        playerInputActions.Memory.Disable();
        playerInputActions.Player.OpenTV.Enable();
        SceneManagement sceneManagement = FindObjectOfType<SceneManagement>();
        sceneManagement.ExitMemoryScene();
    }


    #endregion

    #region Object Interactions
    private void ObjectInteract(InputAction.CallbackContext context)
    {
        //Debug.Log("Interaction button pressed");
        InteractableDetector interactableDetector = GetComponent<InteractableDetector>();
        interactableDetector.InteractWithObject();
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
        }
        else
        {
            Debug.Log("Toggling Off Inspect");
            _interactionCue.SetInteractionCue(InteractionCueType.Hold);
            playerInputActions.Player.Enable();
            playerInputActions.Inspect.Disable();
            inspection.ToggleFocusObject(false);
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
    }

    void SwitchBranchingItem(InputAction.CallbackContext ctx)
    {
        Debug.Log("Switching branching item");
        InteractableDetector interactableDetect = GetComponent<InteractableDetector>();
        interactableDetect.unhighlightObject(currSelectedBranching);
        GameObject otherBranching = currSelectedBranching.GetComponent<PuzzleBranchingKeyItem>().otherBranchingItem;
        interactableDetect.highlightObject(otherBranching);

        currSelectedBranching = otherBranching;
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
    }

    public bool isInBranchingSelection()
    {
        return inBranchingSelection;
    }
    #endregion
}
