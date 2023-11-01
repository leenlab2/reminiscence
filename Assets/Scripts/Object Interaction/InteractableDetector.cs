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

    RaycastHit hit;
    
    private float sphereRadius = 0.2f;
    private GameObject currentObj = null;
    
    private void Start()
    {
        _interactionCue = GameObject.Find("InteractionCue").GetComponent<InteractionCue>();
        
    }


    /*void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(hit.point, sphereRadius);
        Debug.Log("IN");
    }*/

    private void Update()
    {
        Vector3 origin = Camera.main.transform.position;
        _currentHit = null;
            
        NotDetected();

        // Define the ray we are using to detect objects
        origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.TransformDirection(Vector3.forward);
        Ray originRay = new Ray(origin, direction);
        
        
        Debug.DrawRay(origin, direction * maxPlayerReach, Color.red);

        int layerMask = ~LayerMask.GetMask("Ignore Raycast");
        
        // Perform raycast
        if (Physics.SphereCast(originRay, sphereRadius, out hit, maxPlayerReach, layerMask))
           //if (Physics.Raycast(origin, direction, out hit, maxPlayerReach, layerMask))
        {
            // Draw the ray
            Debug.DrawRay(origin, direction * hit.distance, Color.yellow);

            // Draw the sphere at the hit point
            // Draw a yellow sphere at the transform's position
            //OnDrawGizmosSelected();
            
            OnCursorHitChange?.Invoke(hit);
            // Debug.Log("raycast hit: " + hit.transform.gameObject.name);

            CheckInteractableTypeHit(hit);

            if (interactionType != InteractionType.None && hit.transform.parent.name != "TV" && hit.transform.parent.name != "TV_textures")
            {
                Detected();
                _currentHit = hit;
                if (currentObj) // if cursor on interactable object immediately after being on another interactable object
                {
                    Debug.Log(currentObj.name);
                    currentObj.GetComponent<Outline>().OutlineWidth = 0f;
                    currentObj = null;
                }
                currentObj = _currentHit.Value.transform.gameObject;
                currentObj.GetComponent<Outline>().OutlineWidth = 5f;
                currentObj.GetComponent<Outline>().OutlineColor = Color.yellow;
                Debug.Log(currentObj.name);
            }
            else if (currentObj)
            {
                Debug.Log(currentObj.name);
                currentObj.GetComponent<Outline>().OutlineWidth = 0f;
                currentObj = null;
            }
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
        _crossHairDisplay.rectTransform.sizeDelta = new Vector2(20, 20);
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
        _crossHairDisplay.rectTransform.sizeDelta = new Vector2(15, 15);
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
