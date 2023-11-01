using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using TMPro;

public enum InteractionType
{
    None,
    Pickup,
    InsertRemoveTape
}

/// <summary>
/// Detects if the player is looking at an interactable object, and distributes input requests to relevant scripts
/// </summary>
public class InteractableDetector : MonoBehaviour
{
    // Inspector fields
    [SerializeField] private float maxPlayerReach = 5.0f;

    [Header("Crosshairs")]
    [SerializeField] private Image _crossHairDisplay;
    [SerializeField] private Sprite _defaultCrosshair;
    [SerializeField] private Sprite _objectDetected;

    // Events
    public static Action<RaycastHit> OnCursorHitChange;

    // private fields
    private RaycastHit? _currentHit = null;
    private InteractionType interactionType;

    private InteractionCue _interactionCue;

    private void Start()
    {
        _interactionCue = GameObject.Find("InteractionCue").GetComponent<InteractionCue>();
    }

    private void Update()
    {
        _currentHit = null;
        RaycastHit hit;
        NotDetected();

        // Define the ray we are using to detect objects
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(origin, direction * maxPlayerReach, Color.red);

        int layerMask = ~LayerMask.GetMask("Ignore Raycast");

        // Perform raycast
        if (Physics.Raycast(origin, direction, out hit, maxPlayerReach, layerMask))
        {
            OnCursorHitChange?.Invoke(hit);
            // Debug.Log("raycast hit: " + hit.transform.gameObject.name);

            CheckInteractableTypeHit(hit);

            if (interactionType != InteractionType.None)
            {
                Detected();
                _currentHit = hit;
            }
        }

        InputManager inputManager = FindObjectOfType<InputManager>();
        if (inputManager.InTVMode())
        {
            _interactionCue.SetInteractionCue(InteractionCueType.Empty);
        }
        if (inputManager.InInspection())
        {
            _interactionCue.SetInteractionCue(InteractionCueType.Inspection);
        }
    }

    /// <summary>
    /// Determine type of interaction possible, if any, based on hit object
    /// </summary>
    private void CheckInteractableTypeHit(RaycastHit hit)
    {
        PickUpInteractor pickUpInteractor = GetComponent<PickUpInteractor>();

        if (hit.transform.parent?.name == "TV" || hit.transform.parent?.name == "TV_textures")
        {
            if (pickUpInteractor.IsHeld("Tape Model"))
            {
                InputManager inputManager = FindObjectOfType<InputManager>();
                _interactionCue.SetInteractionCue(InteractionCueType.InsertTape);
            }
            
            TapeManager tapeManager = FindObjectOfType<TapeManager>();
            if (tapeManager.televisionHasTape())
            {
                _interactionCue.SetInteractionCue(InteractionCueType.RemoveTape);
            }
            interactionType = InteractionType.InsertRemoveTape;
        }
        else if (hit.transform.GetComponent<PickupInteractable>() && !pickUpInteractor.isHoldingObj())
        {
            _interactionCue.SetInteractionCue(InteractionCueType.Pickup);
            interactionType = InteractionType.Pickup;
        } else
        {
            interactionType = InteractionType.None;
        }
    }

    #region Cursor Sprite
    private void Detected()
    {
        _crossHairDisplay.sprite = _objectDetected;
    }

    private void NotDetected()
    {
        PickUpInteractor pickUpInteractor = GetComponent<PickUpInteractor>();
        if (!pickUpInteractor.isHoldingObj())
        {
            _interactionCue.SetInteractionCue(InteractionCueType.Empty);
        }
        if (pickUpInteractor.IsHeld("Tape Model"))
        {
            InputManager inputManager = FindObjectOfType<InputManager>();
            if (!inputManager.InInspection())
            {
                _interactionCue.SetInteractionCue(InteractionCueType.Hold);
            }
        }
        _crossHairDisplay.sprite = _defaultCrosshair;
    }
    #endregion

    /// <summary>
    /// This method handles listening to the input action for interaction with objects,
    /// and delegates tasks to relevant scripts based on interaction type.
    /// </summary>
    public void InteractWithObject()
    {
        PickUpInteractor pickUpInteractor = GetComponent<PickUpInteractor>();
        if (!_currentHit.HasValue && !pickUpInteractor.isHoldingObj()) return;

        // Delegate tasks based on interaction type
        if (interactionType == InteractionType.InsertRemoveTape)
        {
            Debug.Log("Interaction type: tape");
            TapeManager tapeManager = FindObjectOfType<TapeManager>();

            if (pickUpInteractor.IsHeld("Tape Model"))
            {
                tapeManager.insertTape(pickUpInteractor.HeldObj);
            } else if (!pickUpInteractor.isHoldingObj())
            {
                tapeManager.removeTape();
            }   
        } else if (interactionType == InteractionType.Pickup)
        {
            Debug.Log("Interaction type: pickup");
            _interactionCue.SetInteractionCue(InteractionCueType.Hold);
            GameObject obj = _currentHit.Value.transform.gameObject;
            pickUpInteractor.PickupObject(obj);
        }
    }
}
