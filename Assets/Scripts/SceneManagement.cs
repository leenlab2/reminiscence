using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;

public class SceneManagement : MonoBehaviour
{
    private TapeManager tapeManager;
    
    public GameObject effects;
    public GameObject player;
    public GameObject enterMemoryButton;

    private Vector3 _originalPlayerPos;
    private Quaternion _originalPlayerRot;

    private InteractionCue _interactionCue;

    // Start is called before the first frame update
    void Start()
    {
        tapeManager = FindObjectOfType<TapeManager>();
        _interactionCue = GameObject.Find("InteractionCue").GetComponent<InteractionCue>();

        // Set to dim lighting
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientSkyColor = Color.black;
        RenderSettings.ambientEquatorColor = Color.black;
        RenderSettings.ambientGroundColor = Color.black;

        VideoControls.clipWatched += FirstEntry;
    }

    void FirstEntry()
    {
        VideoControls.clipWatched -= FirstEntry;

        enterMemoryButton.SetActive(true);

        // Selects and clicks on the button
        EventSystem.current.SetSelectedGameObject(enterMemoryButton);
        ExecuteEvents.Execute(enterMemoryButton, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
    }

    public void EnterMemoryScene()
    {
        if (tapeManager.televisionHasTape()) // Enter memory scene if TV has tape inserted
        {
            RenderSettings.ambientIntensity = 0.5f;
            effects.SetActive(true);

            FindObjectOfType<InputManager>().EnterMemoryScene();

            _interactionCue.SetInteractionCue(InteractionCueType.EnterMemory);
            
            _originalPlayerPos = player.transform.position;
            _originalPlayerRot = player.transform.rotation;

            player.transform.position = new Vector3(-5.03f, 50f, 4f);
            player.transform.rotation = new Quaternion(0,0,0, 0);
        }
    }

    public void ExitMemoryScene()
    {
        player.transform.SetPositionAndRotation(_originalPlayerPos, _originalPlayerRot);

        _interactionCue.SetInteractionCue(InteractionCueType.ExitMemory);

        effects.SetActive(false);

        RenderSettings.ambientIntensity = 1;

        Vector3 cameraRotationAtTelevision = new Vector3(0, 160, 0);
        player.transform.rotation = Quaternion.Euler(cameraRotationAtTelevision);
    } 
}