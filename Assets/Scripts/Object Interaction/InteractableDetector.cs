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
    Open,
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
    [SerializeField] private Material highlightMaterial;

    private Vector2 _lastCrossHairDisplaySize = new Vector2(15, 15);

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

        // Perform raycast
        if (Physics.SphereCast(originRay, sphereRadius, out hit, maxPlayerReach, layerMask))
        {
            //Debug.DrawRay(origin, direction * hit.distance, Color.yellow);
            //OnDrawGizmosSelected();

            OnCursorHitChange?.Invoke(hit);

            CheckInteractableTypeHit(hit);

            // if cursor on interactable object immediately after being on another interactable object
            if (currentObj)
            {
                //Debug.Log(currentObj.name);
                unhighlightObject(currentObj);
                currentObj = null;
            }

            if (interactionType != InteractionType.None
                && interactionType != InteractionType.Place)
            {
                _currentHit = hit;
                Detected();
            }
        }

        InputManager inputManager = FindObjectOfType<InputManager>();
        if (inputManager.InTVMode())
        {
            _interactionCue.SetInteractionCue(InteractionCueType.Empty);
            _lastCrossHairDisplaySize = _crossHairDisplay.rectTransform.sizeDelta;
            _crossHairDisplay.rectTransform.sizeDelta = new Vector2(0, 0);
        } else {
            _crossHairDisplay.rectTransform.sizeDelta = _lastCrossHairDisplaySize;
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

        if (hit.transform.name == "VHS")
        {
            if (pickUpInteractor.IsHeld("Tape Model"))
            {
                InputManager inputManager = FindObjectOfType<InputManager>();
                _interactionCue.SetInteractionCue(InteractionCueType.InsertTape);
            }

            TapeManager tapeManager = FindObjectOfType<TapeManager>();
            if (tapeManager.televisionHasTape())
            {
                if (!pickUpInteractor.isHoldingObj())
                {
                    _interactionCue.SetInteractionCue(InteractionCueType.RemoveTape);
                }
            }
            interactionType = InteractionType.InsertRemoveTape;
        }
        else if (hit.transform.GetComponent<PickupInteractable>() && !pickUpInteractor.isHoldingObj())
        {
            _interactionCue.SetInteractionCue(InteractionCueType.Pickup);
            interactionType = InteractionType.Pickup;
        }
        else if (pickUpInteractor.isHoldingObj())
        {
            interactionType = InteractionType.Place;
        } else if (hit.transform.GetComponent<OpenInteractable>())
        {
            _interactionCue.SetInteractionCue(InteractionCueType.Pickup); // TODO: change to open
            interactionType = InteractionType.Open;
        }
        else
        {
            interactionType = InteractionType.None;
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
        }
    }

    private void NotDetected()
    {
        PickUpInteractor pickUpInteractor = GetComponent<PickUpInteractor>();
        if (!pickUpInteractor.isHoldingObj())
        {
            InputManager inputManager = FindObjectOfType<InputManager>();
            if (!inputManager.isInBranchingSelection()){
                _interactionCue.SetInteractionCue(InteractionCueType.Empty);
            }
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
            }
            else if (!pickUpInteractor.isHoldingObj())
            {
                tapeManager.removeTape();
            }
        }
        else if (interactionType == InteractionType.Pickup)
        {
            Debug.Log("Interaction type: pickup");
            // _interactionCue.SetInteractionCue(InteractionCueType.Hold);
            GameObject obj = _currentHit.Value.transform.gameObject;
            pickUpInteractor.PickupObject(obj);
        }
        else if (interactionType == InteractionType.Place)
        {
            Debug.Log("Interaction type: place");
            pickUpInteractor.DropHeldObject();
        } else if (interactionType == InteractionType.Open)
        {
            Debug.Log("Interaction type: open");
            GameObject obj = _currentHit.Value.transform.gameObject;
            obj.GetComponent<OpenInteractable>().Open();
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
        obj.GetComponent<Outline>().OutlineWidth = 0f;
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
    