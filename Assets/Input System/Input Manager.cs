using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public PlayerInputActions playerInputActions;

    private Rigidbody playerBody;
    private PlayerInput playerInput;

    private float _mouseSensitivity = 3f;
    
    private float _sprintSpeed = 9f;
    private float _walkSpeed = 7f;
    private float _speed;
    private bool inspectionMode = false;

    private void Awake()
    {
        _speed = _walkSpeed;
        playerBody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Television.Disable();


        playerInputActions.Player.OpenTV.Enable();
        playerInputActions.Player.OpenTV.performed += OpenTelevision;
        playerInputActions.Television.CloseTV.performed += CloseTelevision;

        playerInputActions.Player.Interact.performed += ObjectInteract;
        playerInputActions.Player.InspectionToggle.performed += ObjectInspectionToggle;
        playerInputActions.Player.Place.performed += ObjectPlacementMode;

        playerInputActions.Player.Rotate.Disable();
    }

    #region Player Movement
    private void FixedUpdate()
    {
        MovePlayer();
        MoveCamera();
        ObjectRotation();
        
    }

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
        Vector2 cameraInput = playerInputActions.Player.Look.ReadValue<Vector2>();

        // Move the player to look around left/right when mouse pans left/right
        transform.Rotate(0, cameraInput.x * _mouseSensitivity, 0);

        // Move the camera to look around up/down when mouse pans up/down
        Transform playerCamera = GetComponentInChildren<Camera>().transform;

        /*if ( (playerCamera.localRotation.x > 0.296681613) && (cameraInput.y > 0)) // Center -> Quaternion(0.112553328,0,0,0.993645728) Up -> Quaternion(-0.178272739,0,0,0.983981133)  Down -> Quaternion(0.296681613,0,0,0.954976499)
        {
            cameraInput.y = 0;
        }
        else if((playerCamera.localRotation.x < -0.3) && (cameraInput.y < 0)) //Quaternion(0.647919357,0,0,0.761708915)
        {
            cameraInput.y = 0;
        }*/
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

        ChangeCameraPosition cameraCtrl = GetComponentInChildren<ChangeCameraPosition>();
        cameraCtrl.SwitchToTapeView();
    }

    private void CloseTelevision(InputAction.CallbackContext obj)
    {
        playerInputActions.Television.Disable();
        playerInputActions.Player.Enable();

        ChangeCameraPosition cameraCtrl = GetComponentInChildren<ChangeCameraPosition>();
        cameraCtrl.SwitchToPlayerView();
    }

    #endregion

    #region Object Interactions
    private void ObjectInteract(InputAction.CallbackContext context)
    {
        //Debug.Log("Interaction button pressed");
        InteractableDetector interactableDetector = GetComponent<InteractableDetector>();
        interactableDetector.InteractWithObject();
    }

    private void ObjectPlacementMode(InputAction.CallbackContext context)
    {
        PickUpInteractor pickUpInteractor = GetComponent<PickUpInteractor>();
        pickUpInteractor.ListenForPlacement(context.action);
    }
    #endregion

    #region Object Rotation
    private void ObjectRotation()
    {
        Vector2 rotationInput = playerInputActions.Player.Rotate.ReadValue<Vector2>();
        Inspection inspection = GetComponentInChildren<Inspection>();
        inspection.RotateObject(rotationInput);
    }

    private void ObjectInspectionToggle(InputAction.CallbackContext ctx)
    {
        Inspection inspection = GetComponentInChildren<Inspection>();
        inspectionMode = !inspectionMode;

        if (inspectionMode)
        {
            Debug.Log("Toggling On Inspect");
            playerInputActions.Player.Look.Disable();
            playerInputActions.Player.Move.Disable();
            playerInputActions.Player.Rotate.Enable();

            inspection.ToggleFocusObject(true);
        }
        else
        {
            Debug.Log("Toggling Off Inspect");
            playerInputActions.Player.Look.Enable();
            playerInputActions.Player.Move.Enable();
            playerInputActions.Player.Rotate.Disable();
            inspection.ToggleFocusObject(false);
        }
    }
    #endregion
}
