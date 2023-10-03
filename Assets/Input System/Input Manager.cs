using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Rigidbody playerBody;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 movementInput = playerInputActions.Player.Move.ReadValue<Vector2>();
        float speed = 3f;
        playerBody.velocity = new Vector3(movementInput.x, 0, movementInput.y) * speed;
    }
}
