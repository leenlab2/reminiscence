using System;
using System.Collections;
using System.Collections.Generic;
// using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

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

    private Quaternion originalHoldAreaRotation;

    private TMP_Text _interactText;
    public static Action<GameObject> OnBranchingPickup;

    private InteractionCue _interactionCue;

#region IsHeld
    public bool isHoldingObj()
    {
        return HeldObj != null;
    }

    private bool isHoldingAnything()
    {
        return isHoldingObj() || righthandObj != null || lefthandObj != null;
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
        _interactionCue = GameObject.Find("InteractionCue").GetComponent<InteractionCue>();
    }

    private void OnDestroy()
    {
        InteractableDetector.OnCursorHitChange -= DetermineNewPosition;
    }

    private void ToggleObjectColliders(GameObject obj, bool on)
    {        
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = on;
        }
    }

    #region Pickup
    public void PickupObject(GameObject obj)
    {
        PickupInteractable pickObj = obj.GetComponent<PickupInteractable>();
        if (pickObj == null) return;

        // TODO: once a more robust map switching system is in place, change this
        if (pickObj.GetComponent<PuzzleBranchingKeyItem>() != null && transform.position.y < 40)
        {
            BranchingObjPickup(pickObj.gameObject);
        } else
        {
            PickupObject(pickObj);
        }
    }

    public void PickupObject(PickupInteractable obj)
    {
        NormalObjPickup(obj, holdArea);
        HeldObj = obj.gameObject;
    }

    private void NormalObjPickup(PickupInteractable obj, Transform newPos)
    {
        ResetHoldArea();

        _interactionCue.SetInteractionCue(InteractionCueType.Hold);

        // _interactText.text = "Hold left to aim and click right to place. Press E to inspect.";

        // Fix rigid body settings of target object
        obj.TogglePlacementGuide(true);
        obj.ToggleFreezeBody(true);
        obj.MakeObjSmall();

        // Move to hand
        obj.MoveToHand(newPos);

        ToggleObjectColliders(obj.gameObject, false);
    }

    #region Branching Item Choice
    private void BranchingObjPickup(GameObject obj)
    {    
        PuzzleBranchingKeyItem puzzleItem = obj.GetComponent<PuzzleBranchingKeyItem>();
        GameObject otherBranching = puzzleItem.otherBranchingItem;

        NormalObjPickup(obj.GetComponent<PickupInteractable>(), rightHand);
        NormalObjPickup(otherBranching.GetComponent<PickupInteractable>(), leftHand);

        _interactionCue.SetInteractionCue(InteractionCueType.Branching);

        righthandObj = obj;
        lefthandObj = otherBranching;

        InteractableDetector interactableDetect = GetComponent<InteractableDetector>();
        interactableDetect.highlightObject(obj);
        OnBranchingPickup?.Invoke(obj);
    }

    public void SelectBranchingItem(GameObject obj)
    {
        DropObject(righthandObj);
        DropObject(lefthandObj);

        PickupInteractable pickObj = obj.GetComponent<PickupInteractable>();
        PickupObject(pickObj);

        righthandObj = null;
        lefthandObj = null;
    }
    #endregion
    #endregion

    private void ResetHoldArea()
    {
        HeldObj = null;
        holdArea.transform.rotation = originalHoldAreaRotation;
    }

    #region Object Drop
    public void DropHeldObject()
    {
        DropObject(HeldObj);

        ResetHoldArea();
    }

    void DropObject(GameObject obj)
    {
        ToggleObjectColliders(obj, true);

        PickupInteractable pickObj = obj.GetComponent<PickupInteractable>();
        pickObj.MoveToPlacementGuide();
        pickObj.TogglePlacementGuide(false);
        pickObj.ToggleFreezeBody(false);
        pickObj.MakeObjBig();
    }

    private void DetermineNewPosition(RaycastHit hit)
    {
        if (!isHoldingAnything()) return;

        if (HeldObj != null)
        {
            HeldObj.GetComponent<PickupInteractable>().TransformPlacementGuide(hit);
        } else
        {
            righthandObj.GetComponent<PickupInteractable>().TransformPlacementGuide(hit);
            lefthandObj.GetComponent<PickupInteractable>().TransformPlacementGuide(hit);
        }
    }
    #endregion
}
