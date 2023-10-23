using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpInteractor : MonoBehaviour
{
    // Inspector settings
    [SerializeField] private Transform holdArea;

    public GameObject HeldObj { get; private set; }

    private Transform objOriginalParent;
    private Quaternion originalHoldAreaRotation;
    private Vector3 originalObjScale;

    private GameObject placementGuide = null;

    public bool IsHeld(GameObject? obj)
    {
        return ReferenceEquals(obj, HeldObj);
    }

    public bool IsHeld(String objectName)
    {
        if (HeldObj == null) return false;

        return objectName == HeldObj.name;
    }

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
        Rigidbody objRB = obj.GetComponent<Rigidbody>();
        if (objRB == null) return;

        ResetHoldArea();

        // Fix rigid body settings of target object
        objRB.useGravity = false;
        objRB.isKinematic = true;
        objRB.drag = 10;
        objRB.constraints = RigidbodyConstraints.FreezeRotation;

        MakeObjSmall(obj);

        // Move to hand
        objOriginalParent = obj.transform.parent;
        obj.transform.SetPositionAndRotation(holdArea.position, holdArea.rotation);
        obj.transform.SetParent(holdArea);
        HeldObj = obj;
    }

    private void ResetHoldArea()
    {
        holdArea.transform.rotation = originalHoldAreaRotation;
    }

    #region Object Placement
    public void DropObject()
    {
        // set position, rotation of held obj to placement guide's
        if (placementGuide != null)
        {
            HeldObj.transform.SetPositionAndRotation(placementGuide.transform.position, placementGuide.transform.rotation);
            placementGuide.SetActive(false);
        }

        Rigidbody objRB = HeldObj.GetComponent<Rigidbody>();

        // Reset rigid body settings of held object
        objRB.useGravity = true;
        objRB.isKinematic = false;
        objRB.drag = 1;
        objRB.constraints = RigidbodyConstraints.None;

        // Remove held object from hand
        HeldObj.transform.parent = objOriginalParent;

        MakeObjBig();

        HeldObj = null;
        placementGuide = null;
        ResetHoldArea();
    }

    private void DetermineNewPosition(RaycastHit hit)
    {
        if (HeldObj == null) return;

        if (placementGuide == null)
        {
            placementGuide = objOriginalParent.transform.Find("Placement Guide").gameObject;
            placementGuide.SetActive(true);
        }

        placementGuide.transform.position = hit.point;
        placementGuide.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
    }
    #endregion

    #region Object Size
    void MakeObjBig()
    {
        HeldObj.transform.localScale = originalObjScale;
    }
    void MakeObjSmall(GameObject pickObj)
    {
        originalObjScale = pickObj.transform.localScale;
        pickObj.transform.localScale = pickObj.transform.localScale / 2;
    }
    #endregion
}
