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

    public GameObject _televisionCanvas;

    private VideoControls _videoControls;

    private GameObject _player;

    void Start()
    {        
        // Get Player
        _player = GameObject.Find("Player");
        
        // Get Television and Camera Transforms
        _televisionTransform = GameObject.Find("TV").GetComponent<Transform>();
        _cameraTransform = GetComponent<Transform>();
        
        // Calculate position of camera when it is on television
        _cameraOnTelevisionTransform = new GameObject().transform;
        print(_televisionTransform.position);
        _cameraOnTelevisionTransform.position = _televisionTransform.position + new Vector3(-1.0f, 1.50f, 3.80f);
        
        // Calculate rotation of camera when it is on television
        Vector3 televisionRotation = _televisionTransform.rotation.eulerAngles;
        Vector3 cameraRotationAtTelevision = televisionRotation +  new Vector3(0, 135f, 0);
        _cameraOnTelevisionTransform.rotation = Quaternion.Euler(cameraRotationAtTelevision);
        
        _videoControls = FindObjectOfType<VideoControls>();
    }
    
    /*
     * If:
     *    TODO: player within range of TV and
     *    camera is currently on player and
     *    player presses button T
     * then switch camera to focus on TV
     */
    public void SwitchToTapeView()
    {
        Debug.Log("Switching to tape view");

        // Create copy of camera transform when on player
        _cameraOnPlayerTransform = new GameObject().transform;
        _cameraOnPlayerTransform.position = _cameraTransform.position;
        _cameraOnPlayerTransform.rotation = _cameraTransform.rotation;
        print(_cameraOnPlayerTransform.position);
        print(_cameraOnTelevisionTransform.position);
        
        Camera.main.transform.SetPositionAndRotation(_cameraOnTelevisionTransform.position, _cameraOnTelevisionTransform.rotation);

        _televisionCanvas.SetActive(true);
        _videoControls = FindObjectOfType<VideoControls>();
        _player.GetComponent<MeshRenderer>().enabled = false;
    }
    
    /*
     * If:
     *   camera currently on TV 
     *   player pressed button T
     * then switch camera back onto player
     */

    public void SwitchToPlayerView()
    {
        Debug.Log("Switching to player view");

        _videoControls.Pause(); // pause video in case playing
        
        Camera.main.transform.SetPositionAndRotation(_cameraOnPlayerTransform.position, _cameraOnPlayerTransform.rotation);
        _televisionCanvas.SetActive(false);
        _player.GetComponent<MeshRenderer>().enabled = true;
    }
}
