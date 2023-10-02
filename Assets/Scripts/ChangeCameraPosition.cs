using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeCameraPosition : MonoBehaviour
{
    private Transform _televisionTransform;

    private Transform _cameraTransform;
    private Transform _cameraOnTelevisionTransform;
    private Transform _cameraOnPlayerTransform;
    
    private bool _televisionIsOpen; // true iff on player. Otherwise focused on TV

    private PlayerInput _playerInput;

    private InputActions _actions;
    private InputAction _openTV;

    public GameObject _televisionCanvas;

    private VideoControls _videoControls;

    void Start()
    {
        // Set up Actions for ppening Television and exiting Television
        _playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
        
        _actions = new InputActions();
        
        _actions.Player.OpenTV.Enable();
        _actions.Player.OpenTV.performed += OpenTelevision;
        _actions.Television.CloseTV.Enable();
        _actions.Television.CloseTV.performed += CloseTelevision;
        
        _playerInput.actions.FindActionMap("Player").Enable();
        _playerInput.actions.FindActionMap("Television").Disable();
        
        // Get Television and Camera Transforms
        _televisionTransform = GameObject.Find("TV").GetComponent<Transform>();
        _cameraTransform = GetComponent<Transform>();
        
        // Create copy of camera transform when on player
        _cameraOnPlayerTransform = new GameObject().transform;
        _cameraOnPlayerTransform.position = _cameraTransform.position;
        _cameraOnPlayerTransform.rotation = _cameraTransform.rotation;
        
        // Calculate position of camera when it is on television
        _cameraOnTelevisionTransform = new GameObject().transform;
        _cameraOnTelevisionTransform.position = _televisionTransform.forward + new Vector3(0, 0.96f, 1.6f);
        
        // Calculate rotation of camera when it is on television
        Vector3 cameraRotationAtTelevision = _televisionTransform.rotation.eulerAngles + new Vector3(0, 180, 0);
        _cameraOnTelevisionTransform.rotation = Quaternion.Euler(cameraRotationAtTelevision);
    }
    
    /*
     * If:
     *    TODO: player within range of TV and
     *    camera is currently on player and
     *    player presses button T
     * then switch camera to focus on TV
     */
    private void OpenTelevision(InputAction.CallbackContext obj)
    {
        _televisionIsOpen = true;
        _playerInput.actions.FindActionMap("Player").Disable();
        _playerInput.actions.FindActionMap("Television").Enable();
        
        print("Triggered Open TV");
        
        _cameraOnPlayerTransform = new GameObject().transform;
        _cameraOnPlayerTransform.position = _cameraTransform.position;
        _cameraOnPlayerTransform.rotation = _cameraTransform.rotation;
        
        Camera.main.transform.SetPositionAndRotation(_cameraOnTelevisionTransform.position, _cameraOnTelevisionTransform.rotation);
        _televisionCanvas.SetActive(true);

        _videoControls = FindObjectOfType<VideoControls>();
    }
    
    /*
     * If:
     *   camera currently on TV 
     *   player pressed button G
     * then switch camera back onto player
     */

    private void CloseTelevision(InputAction.CallbackContext obj)
    {
        if (!_televisionIsOpen) return;
        
        _televisionIsOpen = false;
        _playerInput.actions.FindActionMap("Player").Enable();
        _playerInput.actions.FindActionMap("Television").Disable();
        
        _videoControls.Pause(); // pause video in case playing
        
        Camera.main.transform.SetPositionAndRotation(_cameraOnPlayerTransform.position, _cameraOnPlayerTransform.rotation);
        _televisionCanvas.SetActive(false);
        _actions.Television.Enable();

    }
}
