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

    void Start()
    {        
        // Get Television and Camera Transforms
        _televisionTransform = GameObject.Find("TV").GetComponent<Transform>();
        _cameraTransform = GetComponent<Transform>();
        
        // Calculate position of camera when it is on television
        _cameraOnTelevisionTransform = new GameObject().transform;
        _cameraOnTelevisionTransform.position = _televisionTransform.position + new Vector3(-2.0f, 1.70f, -1.43f);
        
        // Vector3(-2.10370814f,0,2.08507345f) -Vector3(-4.03999996f,-0.0960000008f,-2.73000002f)
        // Quaternion(0,0.916122019,0,-0.400899649)
        // Calculate rotation of camera when it is on television
        Vector3 televisionRotation = _televisionTransform.rotation.eulerAngles;
        Vector3 cameraRotationAtTelevision = televisionRotation +  new Vector3(0, 43f, 0);
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

    public void SwitchToPlayerView()
    {
        Debug.Log("Switching to player view");

        _videoControls.Pause(); // pause video in case playing
        
        Camera.main.transform.SetPositionAndRotation(_cameraOnPlayerTransform.position, _cameraOnPlayerTransform.rotation);
        _televisionCanvas.SetActive(false);
    }
}
