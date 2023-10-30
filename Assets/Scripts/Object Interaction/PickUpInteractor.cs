using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Handles the communication between the player and the object. Specifically manages what object is held (`HeldObj`)
/// </summary>
public class PickUpInteractor : MonoBehaviour
{
    // Inspector settings
    [SerializeField] private Transform holdArea;

    public GameObject HeldObj { get; private set; }
    private PickupInteractable pickupObj;

    private Quaternion originalHoldAreaRotation;

    private TMP_Text _interactText;

#region IsHeld
public bool isHoldingObj()
    {
        return HeldObj != null;
    }

    public bool IsHeld(GameObject? obj)
    {
        return ReferenceEquals(obj, HeldObj);
    }

    public bool IsHeld(String objectName)
    {
        if (!isHoldingObj()) return false;

        return objectName == HeldObj.name;
    }
    #endregion

    private void Start()
    {
        originalHoldAreaRotation = holdArea.rotation;
        InteractableDetector.OnCursorHitChange += DetermineNewPosition;

        _interactText = GameObject.Find("Interact Text").GetComponent<TMP_Text>();
    }

    private void ToggleObjectColliders(GameObject obj, bool on)
    {        
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = on;
        }
    }

    public void PickupObject(GameObject obj)
    {
        PickupInteractable pickObj = obj.GetComponent<PickupInteractable>();
        if (pickObj == null) return;

        ResetHoldArea();

        // _interactText.text = "Hold left to aim and click right to place. Press E to inspect.";

        // Fix rigid body settings of target object
        pickObj.ToggleFreezeBody(true);
        pickObj.MakeObjSmall();

        // Move to hand
        pickObj.MoveToHand(holdArea);
        HeldObj = obj;
        pickupObj = pickObj;

        ToggleObjectColliders(obj, false);
    }

    private void ResetHoldArea()
    {
        HeldObj = null;
        pickupObj = null;
        holdArea.transform.rotation = originalHoldAreaRotation;
    }

    #region Object Placement
    public void ActivatePlacementGuide()
    {
        if (isHoldingObj())
        {
            pickupObj.TogglePlacementGuide(true);
        }
    }

    public void DropObject()
    {
        ToggleObjectColliders(HeldObj, true);

        pickupObj.MoveToPlacementGuide();
        pickupObj.ToggleFreezeBody(false);
        pickupObj.MakeObjBig();

        ResetHoldArea();
    }

    private void DetermineNewPosition(RaycastHit hit)
    {
        if (!isHoldingObj()) return;

        pickupObj.TransformPlacementGuide(hit);
    }
    #endregion
}
