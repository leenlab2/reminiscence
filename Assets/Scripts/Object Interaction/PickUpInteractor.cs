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
    private PickupInteractable pickupObj;

    private Quaternion originalHoldAreaRotation;

    private TMP_Text _interactText;
    public static Action<GameObject> OnBranchingPickup;

    private InteractionCue _interactionCue;

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
        _interactionCue = GameObject.Find("InteractionCue").GetComponent<InteractionCue>();
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

        // TODO: once a more robust map switching system is in place, change this
        if (pickObj.GetComponent<PuzzleBranchingKeyItem>() != null && transform.position.y < 40)
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

        _interactionCue.SetInteractionCue(InteractionCueType.Hold);

        // _interactText.text = "Hold left to aim and click right to place. Press E to inspect.";

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

        _interactionCue.SetInteractionCue(InteractionCueType.Branching);

        righthandObj = obj;
        lefthandObj = otherBranching;

        obj.GetComponent<Outline>().OutlineWidth = 5f;
        OnBranchingPickup?.Invoke(obj);
    }

    public void SelectBranchingItem(GameObject obj)
    {
        obj.GetComponent<PickupInteractable>().MakeObjBig();
        NormalObjPickup(obj.GetComponent<PickupInteractable>(), holdArea);
        HeldObj = obj;
        pickupObj = obj.GetComponent<PickupInteractable>();

        // figure out whether obj is righthandobj or lefthandobj
        GameObject otherObj;
        if (obj == righthandObj)
        {
            otherObj = lefthandObj;
        } else
        {
            otherObj = righthandObj;
        }


        // TODO: when drop no longer relies on placement mode, change this
        PickupInteractable otherPickUpObj = otherObj.GetComponent<PickupInteractable>();
        ToggleObjectColliders(otherObj, true);
        otherPickUpObj.transform.SetParent(otherPickUpObj.originalParent);
        otherPickUpObj.ToggleFreezeBody(false);
        otherPickUpObj.MakeObjBig();

        righthandObj = null;
        lefthandObj = null;
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
