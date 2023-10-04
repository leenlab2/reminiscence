using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Rigidbody playerBody;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;

    private float _mouseSensitivity = 3f;
    private float _speed = 3f;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += ObjectInteract;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        MoveCamera();
    }

    private void MovePlayer()
    {
        Vector2 movementInput = playerInputActions.Player.Move.ReadValue<Vector2>();

        // Movement needs to be in local space axis not world space
        Vector3 movement = transform.TransformDirection(new Vector3(movementInput.x, 0, movementInput.y)) * _speed;
        playerBody.velocity = movement;
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

    #region Object Interactions
    private void ObjectInteract(InputAction.CallbackContext context)
    {
        // In the future if we want to use "Interact" for other things, we can add a check here
        DetectPickUp detectPickUp = GetComponent<DetectPickUp>();
        detectPickUp.ToggleHoldObject();
    }
    #endregion
}
