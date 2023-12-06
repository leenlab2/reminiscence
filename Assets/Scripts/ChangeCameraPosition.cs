using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeCameraPosition : MonoBehaviour
{
    private Camera _originalCamera;

    public GameObject _televisionCanvas;
    public Camera tvViewCamera;
    public GameObject HUD;

    private VideoControls _videoControls;
    [SerializeField] private GameObject _playerModel;

    void Start()
    {        
        _videoControls = FindObjectOfType<VideoControls>();
    }

    /*
     * If:
     *    camera is currently on player and
     *    player presses button T
     * then switch camera to focus on TV
     */
    public void SwitchToTapeView()
    {
        _originalCamera = Camera.main;

        _originalCamera.gameObject.SetActive(false);
        tvViewCamera.gameObject.SetActive(true);

        HUD.GetComponent<Canvas>().worldCamera = tvViewCamera;

        _televisionCanvas.SetActive(true);
        _videoControls = FindObjectOfType<VideoControls>();
        
        // Turn off Player model to avoid blocking TV screen
        _playerModel.SetActive(false);
        
    }
    
    /*
     * If:
     *   camera currently on TV 
     *   player pressed button T
     * then switch camera back onto player
     */

    public void SwitchToPlayerView()
    {
        _videoControls.Pause(); // pause video in case playing

        Debug.Log("to player view");

        // if (_originalCamera != null)
        // {
        //     _originalCamera.gameObject.SetActive(true);
        //     tvViewCamera.gameObject.SetActive(false);

        //     HUD.GetComponent<Canvas>().worldCamera = _originalCamera.transform.Find("UI Camera").GetComponent<Camera>();
        // }

        _originalCamera.gameObject.SetActive(true);
        tvViewCamera.gameObject.SetActive(false);

        HUD.GetComponent<Canvas>().worldCamera = _originalCamera.transform.Find("UI Camera").GetComponent<Camera>();

        _televisionCanvas.SetActive(false);
        
        // Turn Player model back on
        _playerModel.SetActive(true);
    }
}
