using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpInteractor : MonoBehaviour
{
    // Inspector settings
    [SerializeField] private Transform holdArea;

    public GameObject HeldObj { get; private set; }
    private PickupInteractable pickupObj;

    private Quaternion originalHoldAreaRotation;

    #region IsHeld
    public bool IsHeld(GameObject? obj)
    {
        return ReferenceEquals(obj, HeldObj);
    }

    public bool IsHeld(String objectName)
    {
        if (HeldObj == null) return false;

        return objectName == HeldObj.name;
    }
    #endregion

    private void Start()
    {
        originalHoldAreaRotation = holdArea.rotation;
        InteractableDetector.OnCursorHitChange += DetermineNewPosition;
    }

    public void ToggleHoldObject(RaycastHit? hit)
    {
        if (hit.HasValue && HeldObj == null)
        {
            GameObject obj = hit.Value.transform.gameObject;
            PickupObject(obj);
        }
        else
        {
            DropObject();
        }
    }

    public void PickupObject(GameObject obj)
    {
        PickupInteractable pickObj = obj.GetComponent<PickupInteractable>();
        if (pickObj == null) return;

        ResetHoldArea();

        // Fix rigid body settings of target object
        pickObj.ToggleFreezeBody(true);
        pickObj.MakeObjSmall();

        // Move to hand
        pickObj.MoveToHand(holdArea);
        HeldObj = obj;
        pickupObj = pickObj;
    }

    private void ResetHoldArea()
    {
        HeldObj = null;
        pickupObj = null;
        holdArea.transform.rotation = originalHoldAreaRotation;
    }

    #region Object Placement
    public void DropObject()
    {
        pickupObj.MoveToPlacementGuide();
        pickupObj.ToggleFreezeBody(false);
        pickupObj.MakeObjBig();

        ResetHoldArea();
    }

    private void DetermineNewPosition(RaycastHit hit)
    {
        if (HeldObj == null) return;

        pickupObj.TransformPlacementGuide(hit);
    }
    #endregion
}
