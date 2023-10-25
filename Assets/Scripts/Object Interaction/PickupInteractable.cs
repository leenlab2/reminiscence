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

    private Transform originalParent;
    private Vector3 originalObjScale;
    private Rigidbody rigidbody;

    void Start()
    {
        originalParent = transform.parent;
        originalObjScale = transform.localScale;
        rigidbody = GetComponent<Rigidbody>();
    }

    public void MoveToHand(Transform holdArea)
    {
        transform.SetPositionAndRotation(holdArea.position, holdArea.rotation);
        transform.SetParent(holdArea);
    }

    public void MoveToPlacementGuide()
    {
        transform.SetPositionAndRotation(placementGuide.transform.position, placementGuide.transform.rotation);
        placementGuide.SetActive(false);
        transform.SetParent(originalParent);
    }

    public void TogglePlacementGuide(bool on)
    {
        placementGuide.SetActive(on);
    }

    public void TransformPlacementGuide(RaycastHit hit)
    {
        if (!wallMountable && hit.normal.y <= 0.1) return;

        placementGuide.transform.position = hit.point;
        placementGuide.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
    }

    public void ToggleFreezeBody(bool freeze)
    {
        rigidbody.useGravity = !freeze;
        rigidbody.isKinematic = freeze;

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
