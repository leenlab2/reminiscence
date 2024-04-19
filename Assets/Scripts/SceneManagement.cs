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
    public Animator cameraAnimator;

    public Transform spawnpoint;

    public AudioClip memoryEnterSfx;
    public AudioClip memoryExitSfx;

    private Vector3 _originalPlayerPos;
    private Quaternion _originalPlayerRot;

    // Start is called before the first frame update
    void Start()
    {
        tapeManager = FindObjectOfType<TapeManager>();
        
        // Set to dim lighting
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientSkyColor = Color.black;
        RenderSettings.ambientEquatorColor = Color.black;
        RenderSettings.ambientGroundColor = Color.black;

        VideoControls.clipWatched += FirstEntry;
    }

    private void OnDestroy()
    {
        VideoControls.clipWatched -= FirstEntry;
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
            Debug.Log("Entering memory scene");
            cameraAnimator.SetTrigger("Enter");

            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.pitch = 1;
            GetComponent<AudioSource>().PlayOneShot(memoryEnterSfx);
            AudioController.ActivateFilters();
        }
    }

    public void SetupMemoryScene()
    {
        TVReactionVoicelineManager.instance.Stop();

        // RenderSettings.ambientIntensity = 0.5f;
        effects.SetActive(true);

        InputManager.instance.EnterMemoryScene();

        _originalPlayerPos = player.transform.position;
        _originalPlayerRot = player.transform.rotation;

        MovePlayerToScene(spawnpoint.position, spawnpoint.rotation);
    }

    public void ExitMemoryScene()
    {
        AudioController.ActivateFilters(false);  
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.pitch = 3;
        GetComponent<AudioSource>().PlayOneShot(memoryExitSfx);

        MovePlayerToScene(_originalPlayerPos, _originalPlayerRot);

        effects.SetActive(false);

        // RenderSettings.ambientIntensity = 0.5f;

        Vector3 cameraRotationAtTelevision = new Vector3(0, 160, 0);
        player.transform.rotation = Quaternion.Euler(cameraRotationAtTelevision);
    } 

    private void MovePlayerToScene(Vector3 newSpawnPointPos, Quaternion newRotation)
    {
        player.transform.SetPositionAndRotation(newSpawnPointPos, newRotation);

        // if is holding object, move to scene
        PickUpInteractor pickUpInteractor = player.GetComponent<PickUpInteractor>();
        if (pickUpInteractor.isHoldingObj())
        {
            pickUpInteractor.HeldObj.GetComponent<PickupInteractable>().MovePlacementGuideToScene(newSpawnPointPos);
        }
    }
}