using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.VisualScripting;
using static UnityEngine.Rendering.DebugUI.Table;

public enum InteractionType
{
    None,
    Pickup,
    Place,
    PlaceInContainer,
    Open,
    Close,
    InsertTape,
    RemoveTape,
    SwapTape,
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
    [SerializeField] private Material highlightMaterial;

    private Vector2 _lastCrossHairDisplaySize = new Vector2(15, 15);

    // Events
    public static Action<RaycastHit> OnCursorHitChange;

    // private fields
    private RaycastHit? _currentHit = null;
    private Transform _currentInteractable = null;  // what the player looked at when the interaction type was determined
    public static InteractionType interactionType { get; private set; } = InteractionType.None;
    

    RaycastHit hit;

    private float sphereRadius = 0.2f;
    private GameObject currentObj = null;


    /*void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(hit.point, sphereRadius);
        Debug.Log("IN");
    }*/

    private void FixedUpdate()
    {
        Vector3 origin = Camera.main.transform.position;

        NotDetected();

        // Define the ray we are using to detect objects
        origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.TransformDirection(Vector3.forward);
        Ray originRay = new Ray(origin, direction);
        
        Debug.DrawRay(origin, direction * maxPlayerReach, Color.red);

        int layerMask = ~LayerMask.GetMask("Ignore Raycast");
        
        // Unhighlight VHS player if it has a tape
        TapeManager tapeManager = FindObjectOfType<TapeManager>();
        if (tapeManager.televisionHasTape())
        {
            GameObject vhsPlayer = GameObject.Find("VHS");
            unhighlightObject(vhsPlayer);
        }

        // Perform raycast
        if (Physics.SphereCast(originRay, sphereRadius, out hit, maxPlayerReach, layerMask))
        {
            //Debug.DrawRay(origin, direction * hit.distance, Color.yellow);
            //OnDrawGizmosSelected();

            // Debug.Log(hit.transform.name);
            OnCursorHitChange?.Invoke(hit);

            CheckInteractableTypeHit(hit);
            
            // if cursor on interactable object immediately after being on another interactable object
            if (currentObj)
            {
                //Debug.Log(currentObj.name);

                // If cursor moves off VHS player and TV has no tape, keep VHS player highlighted. Otherwise, unhighlight object
                if (!(currentObj.name == "VHS" && !tapeManager.televisionHasTape()))
                {
                    unhighlightObject(currentObj);
                }
                currentObj = null;
            }

            if (interactionType != InteractionType.None
                && interactionType != InteractionType.Place)
            {
                _currentHit = hit;
                Detected();
            }
        }

        if (InputManager.instance.InTVMode())
        {
            _lastCrossHairDisplaySize = _crossHairDisplay.rectTransform.sizeDelta;
            _crossHairDisplay.rectTransform.sizeDelta = new Vector2(0, 0);
        } else {
            _crossHairDisplay.rectTransform.sizeDelta = _lastCrossHairDisplaySize;
        }
    }

    /// <summary>
    /// Determine type of interaction possible, if any, based on hit object
    /// </summary>
    private void CheckInteractableTypeHit(RaycastHit hit)
    {
        _currentInteractable = hit.transform;
        PickUpInteractor pickUpInteractor = GetComponent<PickUpInteractor>();

        if (hit.transform.name == "VHS")
        {
            TapeManager tapeManager = FindObjectOfType<TapeManager>();
            if (pickUpInteractor.IsHeld("Tape Model") && tapeManager.televisionHasTape())
            {
                interactionType = InteractionType.SwapTape;                
            } 
            else if (pickUpInteractor.IsHeld("Tape Model") && !tapeManager.televisionHasTape())
            {
                interactionType = InteractionType.InsertTape;
            }
            else if (!pickUpInteractor.isHoldingObj() && tapeManager.televisionHasTape())
            {
                interactionType = InteractionType.RemoveTape;
            }
        }
        else if (pickUpInteractor.isHoldingObj())
        {
            InputManager.instance.EnableInteract();
            interactionType = InteractionType.Place;
        }
        else if (hit.transform.GetComponent<Interactable>()?.isInteractable ?? false)
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();

            switch (interactable)
            {
                case Container openInteractable:
                    bool isOpen = hit.transform.GetComponent<Container>().isOpen;
                    interactionType = isOpen ? InteractionType.Close : InteractionType.Open;
                    break;
                case PickupInteractable pickupInteractable when !pickUpInteractor.isHoldingObj():
                    interactionType = InteractionType.Pickup;
                    break;
                default:
                    break;
            }

        }
        else
        {
            interactionType = InteractionType.None;
            _currentInteractable = null;
        }
    }

    #region Cursor Sprite
    private void Detected()
    {
        _crossHairDisplay.sprite = _objectDetected;
        _crossHairDisplay.rectTransform.sizeDelta = new Vector2(20, 20);

        if (currentObj ==null )
        {
            currentObj = _currentHit.Value.transform.gameObject;
            highlightObject(currentObj);
            InputManager.instance.EnableInteract();
        }
    }

    private void NotDetected()
    {
        PickUpInteractor pickUpInteractor = GetComponent<PickUpInteractor>();

        _crossHairDisplay.sprite = _defaultCrosshair;
        _crossHairDisplay.rectTransform.sizeDelta = new Vector2(15, 15);

        InputManager.instance.DisableInteract();
    }
    #endregion

    /// <summary>
    /// This method handles listening to the input action for interaction with objects,
    /// and delegates tasks to relevant scripts based on interaction type.
    /// </summary>
    public void InteractWithObject()
    {
        if (interactionType == InteractionType.None) return;

        TapeManager tapeManager = FindObjectOfType<TapeManager>();
        PickUpInteractor pickUpInteractor = GetComponent<PickUpInteractor>();
        if (!_currentHit.HasValue && !pickUpInteractor.isHoldingObj()) return;

        // Delegate tasks based on interaction type
        if (interactionType == InteractionType.InsertTape)
        {
            Debug.Log("Interaction type: tape insert");
            tapeManager.insertTape(pickUpInteractor.HeldObj);
        }
        else if (interactionType == InteractionType.RemoveTape)
        {
            Debug.Log("Interaction type: tape remove");
            tapeManager.removeTape();
        }
        else if (interactionType == InteractionType.SwapTape)
        {
            Debug.Log("Interaction type: tape swap");
            tapeManager.swapTape(pickUpInteractor.HeldObj);
        }
        else if (interactionType == InteractionType.Pickup)
        {
            Debug.Log("Interaction type: pickup");
            GameObject obj = _currentHit.Value.transform.gameObject;
            pickUpInteractor.PickupObject(obj);
        }
        else if (interactionType == InteractionType.Place)
        {
            // check if the raycast hit is container
            if (_currentInteractable.GetComponent<Container>()?.isOpen ?? false)
            {
                Debug.Log("Interaction type: place in container");
                GameObject container = _currentInteractable.gameObject;
                pickUpInteractor.DropHeldObject(container.GetComponent<Container>());
            } else
            {
                Debug.Log("Interaction type: place");
                pickUpInteractor.DropHeldObject();
            }
        }
        else if (interactionType == InteractionType.Open || interactionType == InteractionType.Close)
        {
            Debug.Log("Interaction type: open/close");
            GameObject obj = _currentInteractable.gameObject;
            obj.GetComponent<Container>().ToggleOpen();
        }
    }

    public void highlightObject(GameObject obj)
    {
        obj.GetComponent<Outline>().OutlineWidth = 5f;
        obj.GetComponent<Outline>().OutlineColor = Color.yellow;
        AddMaterial(obj);
    }

    public void unhighlightObject(GameObject obj)
    {
        if (!GetComponent<PickUpInteractor>().IsHeld(obj))
        {
            obj.GetComponent<Outline>().OutlineWidth = 0f; 
        }

        RemoveMaterial(obj);
    }

    private void AddMaterial(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            // get a list of the materials assigned to renderer
            List<Material> myMaterials = renderer.sharedMaterials.ToList();
            // if highlight material is not in myMaterials
            if (renderer.sharedMaterial != highlightMaterial)
            {
                myMaterials.Add(highlightMaterial);
                renderer.materials = myMaterials.ToArray();
            }
        }
    }

    private void RemoveMaterial(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            List<Material> myMaterials = renderer.sharedMaterials.ToList();
            myMaterials.Remove(highlightMaterial);
            renderer.materials = myMaterials.ToArray();
        }
    }
}
    