using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the functions for modifying the object as well as managing its placement guide
/// </summary>
public class PickupInteractable : Interactable
{
    [Header("Pickup/Place Sound Effects")]
    // TODO: change this to use enterSound and exitSound
    // Use FormerlySerializedAs to keep the old names for the sound effects
    public AudioSource pickupSound;
    public AudioSource placeSound;

    [Header("Placement")]
    [SerializeField] private GameObject placementGuide = null;
    [SerializeField] private bool wallMountable = false;

    private Transform originalParent;
    private Vector3 originalObjScale;
    private Rigidbody rigidbody;
    private bool guideOnWall;
    private bool onWall;

    [Header("Inspection Dialogue")]
    public string inspectionObjectText;
    public AudioClip dialogueAudio;

    void Awake()
    {
        base.Awake();
        originalParent = transform.parent;
        originalObjScale = transform.localScale;
        rigidbody = GetComponent<Rigidbody>();
        onWall = false;
        guideOnWall = false;
    }

    public void MoveToHand(Transform holdArea, Camera pickupCamera)
    {
        // convert the hold area position to screen space
        Vector3 screenPos = Camera.main.WorldToScreenPoint(holdArea.position);

        // scale camera according to object size
        Bounds objBounds = Inspection.GetObjectBounds(transform);
        float largestDim = Mathf.Max(objBounds.size.x, objBounds.size.y, objBounds.size.z);
        pickupCamera.orthographicSize = largestDim * 2;

        // move object to hold area
        Vector3 camPos = pickupCamera.ScreenToWorldPoint(screenPos);
        Vector3 offset = transform.position - objBounds.center;

        transform.SetPositionAndRotation(camPos + offset, holdArea.rotation);
        transform.SetParent(holdArea);
        Inspection.ChangeObjectLayer(transform, "Pickup");

        // Update trackers
        onWall = false;
        guideOnWall = false;

        // Add outline
        GetComponent<Outline>().OutlineWidth = 5f;
        GetComponent<Outline>().OutlineColor = Color.grey;

        ObjectVoicelineManager.instance.SetDialogue(inspectionObjectText, dialogueAudio);

        if (pickupSound.clip != null)
        {
            pickupSound.Play();
        }
    }

    public void MoveToContainer(Container container)
    {
        MoveToPlacementGuide();
        transform.SetParent(container.attachPoint);
    }

    #region Placement Guide
    public void TogglePlacementGuide(bool on)
    {
        placementGuide.SetActive(on);
    }

    public void ResetParent()
    {
        transform.SetParent(originalParent);
    }

    public void MoveToPlacementGuide()
    {
        GetComponent<Outline>().OutlineWidth = 0f;
        Inspection.ChangeObjectLayer(transform, "Default");
        transform.SetPositionAndRotation(placementGuide.transform.position, placementGuide.transform.rotation);
        transform.SetParent(originalParent);

        if (placeSound.clip != null)
        {
            placeSound.Play();
        }

        if (wallMountable && guideOnWall)
        {
            onWall = true;
        }
    }

    public void MovePlacementGuideToScene(Vector3 newSpawnPoint)
    {
        placementGuide.transform.position = newSpawnPoint + new Vector3(0, 0, 1);
    }

    public void TransformPlacementGuide(RaycastHit hit)
    {
        if (!placementGuide.activeSelf) return;

        onWall = false;
        guideOnWall = false;
        bool hitIsWall;
        if(!IsValidSurface(hit, out hitIsWall)) return;

        placementGuide.transform.position = hit.point;

        if (wallMountable || !hitIsWall)
        {
            placementGuide.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }

        if (wallMountable && hitIsWall) {
            guideOnWall = true;
            placementGuide.transform.position += 0.1f * hit.normal;
        } else if (!wallMountable && hitIsWall)
        {
            placementGuide.transform.position += hit.normal;
        }
    }

    private bool IsValidSurface(RaycastHit hit, out bool hitIsWall)
    {
        hitIsWall = hit.normal.y <= 0.05f;

        // get height of object
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; ++i)
        {
            bounds.Encapsulate(renderers[i].bounds.min);
            bounds.Encapsulate(renderers[i].bounds.max);
        }

        float height = bounds.max.y - bounds.min.y;

        float topOfObject = hit.point.y + height;

        bool hitIsRoofInAttic = 7 <= topOfObject && topOfObject <= 50;
        bool hitIsRoofInMemory = 57 <= topOfObject;
        bool hitIsRoof = hitIsRoofInAttic || hitIsRoofInMemory;

        float distFromFlat = Vector3.Distance(hit.normal, new Vector3(0f, 1f, 0f));
        bool hitIsFloor = distFromFlat <= 0.05f;

        return (hitIsFloor || hitIsWall) && !hitIsRoof;
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
        // TODO: figure out how to make all held objects a consistent size
    }
    #endregion
}
