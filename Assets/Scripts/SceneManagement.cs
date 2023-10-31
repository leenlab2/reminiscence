using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneManagement : MonoBehaviour
{
    private InputManager inputManager;
    private TapeManager tapeManager;
    private PuzzleManagerNew puzzleManager;
    public GameObject effects;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
        tapeManager = FindObjectOfType<TapeManager>();
        puzzleManager = FindObjectOfType<PuzzleManagerNew>();
    }

    public void EnterMemoryScene()
    {
        if (tapeManager.televisionHasTape()) // Enter memory scene if TV has tape inserted
        {
            inputManager.CloseTelevision(new InputAction.CallbackContext());
            inputManager.playerInputActions.FindAction("ExitMemoryScene").Enable();
            
            player.transform.position = new Vector3(-5.03f, 50f, 4f);
            player.transform.rotation = new Quaternion(0,0,0, 0);
            
            effects.SetActive(true);

            // if a branching item is placed, show the shadow cues of the 3 key items
            if (puzzleManager.currentBranch != Branch.None)
            {
                puzzleManager.ShowNonBranchingItemsShadowCues();
            }

        }
    }
    
    public void ExitMemoryScene()
    {
        inputManager.playerInputActions.FindAction("ExitMemoryScene").Disable();
        player.transform.position = new Vector3(6.5f, -0.00115942955f, -9.0f);
        player.transform.rotation = new Quaternion(-1.7f,-0.95f,8.96f, 0);
        
        puzzleManager.memorySceneCanvas.SetActive(false);
        effects.SetActive(false);

        Vector3 cameraRotationAtTelevision = new Vector3(0, 160, 0);
        player.transform.rotation = Quaternion.Euler(cameraRotationAtTelevision);
    }
}