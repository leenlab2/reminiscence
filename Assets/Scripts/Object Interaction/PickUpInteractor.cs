using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

/// <summary>
/// Handles the communication between the player and the object. Specifically manages what object is held (`HeldObj`)
/// </summary>
public class PickUpInteractor : MonoBehaviour
{
    // Inspector settings
    [SerializeField] private Transform holdArea;

    [Header("Branching Item Held Position")]
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform leftHand;

    public GameObject HeldObj { get; private set; }
    private GameObject righthandObj;
    private GameObject lefthandObj;
    private PickupInteractable pickupObj;

    private Quaternion originalHoldAreaRotation;

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

        if (pickObj.GetComponent<PuzzleBranchingKeyItem>() != null)
        {
            BranchingObjPickup(pickObj.gameObject);
        } else
        {
            NormalObjPickup(pickObj, holdArea);
        }

        HeldObj = obj;
        pickupObj = pickObj;
    }

    private void NormalObjPickup(PickupInteractable obj, Transform newPos)
    {
        ResetHoldArea();

        // Fix rigid body settings of target object
        obj.ToggleFreezeBody(true);
        obj.MakeObjSmall();

        // Move to hand
        obj.MoveToHand(newPos);

        ToggleObjectColliders(obj.gameObject, false);
    }

    private void BranchingObjPickup(GameObject obj)
    {
        PuzzleBranchingKeyItem puzzleItem = obj.GetComponent<PuzzleBranchingKeyItem>();
        GameObject otherBranching = puzzleItem.otherBranchingItem;

        NormalObjPickup(obj.GetComponent<PickupInteractable>(), rightHand);
        NormalObjPickup(otherBranching.GetComponent<PickupInteractable>(), leftHand);

        righthandObj = obj;
        lefthandObj = otherBranching;
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
