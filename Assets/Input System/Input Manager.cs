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
    private float _speed = 3f;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Television.Disable();


        playerInputActions.Player.OpenTV.Enable();
        playerInputActions.Player.OpenTV.performed += OpenTelevision;
        playerInputActions.Television.CloseTV.performed += CloseTelevision;

        playerInputActions.Player.Interact.performed += ObjectInteract;
        playerInputActions.Player.RotateToggle.performed += ctx => ObjectRotateToggle(ctx.ReadValue<float>()); ;

        //playerInputActions.Player.RotateToggle2.Disable();
        //playerInputActions.Player.RotateToggle.cancelled += ObjectRotateOff;

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
        }
    }

    private void MoveCamera()
    {
        Vector2 cameraInput = playerInputActions.Player.Look.ReadValue<Vector2>();

        // Move the player to look around left/right when mouse pans left/right
        transform.Rotate(0, cameraInput.x * _mouseSensitivity, 0);

        // Move the camera to look around up/down when mouse pans up/down
        Transform playerCamera = GetComponentInChildren<Camera>().transform;
        playerCamera.Rotate(-cameraInput.y * _mouseSensitivity, 0, 0);
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
        // In the future if we want to use "Interact" for other things, we can add a check here
        DetectPickUp detectPickUp = GetComponent<DetectPickUp>();
        detectPickUp.ToggleHoldObject();
    }
    #endregion

    #region Object Rotation
    private void ObjectRotation()
    {
        Vector2 rotationInput = playerInputActions.Player.Rotate.ReadValue<Vector2>();
        Transform heldObjectRotation = GetComponentInChildren<Camera>().transform.GetChild(0).gameObject.transform;
        heldObjectRotation.Rotate(rotationInput.y, rotationInput.x, 0);
    }

   /* private void ObjectRotateOn(InputAction.CallbackContext context)
    {

        *//*bool pressedDown = context.ReadValueAsButton();
        if (pressedDown) // the key has been pressed
        {
            //dothething
            Debug.Log("Toggling On Rotate");
            playerInputActions.Player.Look.Disable();
            playerInputActions.Player.Rotate.Enable();
        }
        if (!pressedDown) //the key has been released
        {
            //stopdoingthething
            Debug.Log("Toggling Off Rotate");
            playerInputActions.Player.Look.Enable();
            playerInputActions.Player.Rotate.Disable();
        }*//*

        Debug.Log("Toggling On Rotate");
        playerInputActions.Player.Look.Disable();
        playerInputActions.Player.Rotate.Enable();
        //playerInputActions.Player.RotateToggle.Disable();
        //playerInputActions.Player.RotateToggle2.Enable();
    }
    private void ObjectRotateOff(InputAction.CallbackContext context)
    {
    
        Debug.Log("Toggling Off Rotate");
        playerInputActions.Player.Look.Enable();
        playerInputActions.Player.Rotate.Disable();
        //playerInputActions.Player.RotateToggle2.Disable();
        //playerInputActions.Player.RotateToggle.Enable();
    }*/

    private void ObjectRotateToggle(float b)
    {
        if (b > 0)
        {
            //LMB = true;
            Debug.Log("Toggling On Rotate");
            playerInputActions.Player.Look.Disable();
            playerInputActions.Player.Rotate.Enable();
        }
        else
        {
            //LMB = false;
            Debug.Log("Toggling Off Rotate");
            playerInputActions.Player.Look.Enable();
            playerInputActions.Player.Rotate.Disable();
        }
    }

    #endregion
}
