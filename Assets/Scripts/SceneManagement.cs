using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class SceneManagement : MonoBehaviour
{
    private InputManager inputManager;
    private TapeManager tapeManager;

    private VideoControls videoControls;
    private PuzzleManagerNew puzzleManager;
    
    public GameObject effects;
    public GameObject player;

    private GameObject camera;
    private bool firstExit = true;

    public bool automaticallyEnterMemorySceneOnOpenTV;

    private InteractionCue _interactionCue;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        tapeManager = FindObjectOfType<TapeManager>();
        tapeManager = FindObjectOfType<TapeManager>();
        videoControls = FindObjectOfType<VideoControls>();
        puzzleManager = FindObjectOfType<PuzzleManagerNew>();
        _interactionCue = GameObject.Find("InteractionCue").GetComponent<InteractionCue>();
        camera = Camera.main.gameObject;
        automaticallyEnterMemorySceneOnOpenTV = false;
    }

    public void EnterMemoryScene()
    {
        if (tapeManager.televisionHasTape()) // Enter memory scene if TV has tape inserted
        {
            effects.SetActive(true);
            camera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
            
            inputManager.CloseTelevision(new InputAction.CallbackContext());
            inputManager.playerInputActions.FindAction("ExitMemoryScene").Enable();
            inputManager.playerInputActions.FindAction("OpenTV").Disable();

            _interactionCue.SetInteractionCue(InteractionCueType.EnterMemory);
            
            player.transform.position = new Vector3(-5.03f, 50f, 4f);
            player.transform.rotation = new Quaternion(0,0,0, 0);
        }
    }
    
    public void ExitMemoryScene()
    {
        inputManager.playerInputActions.FindAction("ExitMemoryScene").Disable();
        inputManager.playerInputActions.FindAction("OpenTV").Enable();
        player.transform.position = new Vector3(6.5f, -0.00115942955f, -9.0f);
        player.transform.rotation = new Quaternion(-1.7f,-0.95f,8.96f, 0);

        _interactionCue.SetInteractionCue(InteractionCueType.ExitMemory);
        
        puzzleManager.memorySceneCanvas.SetActive(false);
        effects.SetActive(false);
        camera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = false;

        Vector3 cameraRotationAtTelevision = new Vector3(0, 160, 0);
        player.transform.rotation = Quaternion.Euler(cameraRotationAtTelevision);

        // if a branching item is placed, show the shadow cues of the 3 key items
        if (firstExit && puzzleManager.currentBranch != Branch.None)
        {
            firstExit = false;
            puzzleManager.ShowNonBranchingItemsShadowCues();
        }
    }

    public void EnableAutomaticEnterMemoryScene()
    {
        // if first time player enters memory scene, automatically teleport them in when they open TV
        if (puzzleManager.level == 1 && tapeManager.televisionHasTape())
        {
            automaticallyEnterMemorySceneOnOpenTV = true;
            
            // draw player's attention to TV for the first time after selecting branching object
            videoControls.televisionParticleEffects.startColor = Color.white;
            videoControls.televisionParticleEffects.Play();
            videoControls.televisionAudioSource.Play();
        }
    }

    public void DisableAutomaticEnterMemoryScene()
    {
        automaticallyEnterMemorySceneOnOpenTV = false;
    }
    
}