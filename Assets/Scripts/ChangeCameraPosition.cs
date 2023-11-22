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

    void Start()
    {        
        _videoControls = FindObjectOfType<VideoControls>();
        InputManager.OnGamePaused += PauseGame;
        InputManager.OnGameResumed += ResumeGame;
    }

    private void OnDestroy()
    {
        InputManager.OnGamePaused -= PauseGame;
        InputManager.OnGameResumed -= ResumeGame;
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

        _originalCamera = Camera.main;

        _originalCamera.gameObject.SetActive(false);
        tvViewCamera.gameObject.SetActive(true);

        HUD.GetComponent<Canvas>().worldCamera = tvViewCamera;

        _televisionCanvas.SetActive(true);
        _videoControls = FindObjectOfType<VideoControls>();
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

        _originalCamera.gameObject.SetActive(true);
        tvViewCamera.gameObject.SetActive(false);

        HUD.GetComponent<Canvas>().worldCamera = _originalCamera.transform.Find("UI Camera").GetComponent<Camera>();

        _televisionCanvas.SetActive(false);
    }

    void PauseGame()
    {
        if (_televisionCanvas.activeSelf)
        {
            _televisionCanvas.GetComponent<UnityEngine.EventSystems.EventSystem>().enabled = false;
        }
    }

    void ResumeGame()
    {
        if (_televisionCanvas.activeSelf)
        {
            _televisionCanvas.GetComponent<UnityEngine.EventSystems.EventSystem>().enabled = true;
        }
    }
}
