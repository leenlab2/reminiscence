using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the functions for modifying the object as well as managing its placement guide
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PickupInteractable : MonoBehaviour
{
    [SerializeField] private GameObject placementGuide = null;
    [SerializeField] private bool wallMountable = false;

    public Transform originalParent;
    private Vector3 originalObjScale;
    private Rigidbody rigidbody;
    private bool onWall;

    void Awake()
    {
        originalParent = transform.parent;
        originalObjScale = transform.localScale;
        rigidbody = GetComponent<Rigidbody>();
    }

    public void MoveToHand(Transform holdArea)
    {
        transform.SetPositionAndRotation(holdArea.position, holdArea.rotation);
        transform.SetParent(holdArea);
        onWall = false;
    }

    #region Placement Guide
    public void MoveToPlacementGuide()
    {
        transform.SetPositionAndRotation(placementGuide.transform.position, placementGuide.transform.rotation);
        transform.SetParent(originalParent);
    }

    public void TransformPlacementGuide(RaycastHit hit)
    {
        if (!placementGuide.activeSelf) return;

        onWall = false;
        bool hitIsWall = hit.normal.y <= 0.05f;
        float distFromFlat = Vector3.Distance(hit.normal, new Vector3(0f, 1f, 0f));
        bool hitIsFloor = distFromFlat <= 0.05f;

        if (!wallMountable && hitIsWall || (!hitIsFloor && !hitIsWall)) { return; }

        placementGuide.transform.position = hit.point;
        placementGuide.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

        if (wallMountable && hitIsWall) { 
            onWall = true;
            placementGuide.transform.position += 0.1f * hit.normal;
        }
    }

    public void ToggleFreezeBody(bool freeze)
    {
        if (wallMountable && onWall) {
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
        } 
        else {
            //Debug.Log("Rigid Body: " + rigidbody);
            //Debug.Log(freeze);
            rigidbody.useGravity = !freeze;
            rigidbody.isKinematic = freeze;
        }

        if (freeze)
        {
            rigidbody.drag = 10;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        } else
        {
            rigidbody.drag = 1;
            rigidbody.constraints = RigidbodyConstraints.None;
        }
    }
    #endregion

    public void DisableWallMountable()
    {
        wallMountable = false;
    }

    #region Object Size
    // These are used to make the object smaller in the HUD when it is held
    public void MakeObjBig()
    {
        transform.localScale = originalObjScale;
    }
    public void MakeObjSmall()
    {
        originalObjScale = transform.localScale;
        transform.localScale = transform.localScale / 2;
    }
    #endregion
}
