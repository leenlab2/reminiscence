using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constantly calculates distance between this object and its target object.
/// If object is in correct location, trigger PuzzleItem <KeyItemPlaced> event.
/// If object is not in correct location, trigger PuzzleItem <KeyItemRemoved> event.
/// </summary>
public class ObjectDistanceNew : MonoBehaviour
{
    public GameObject targetObj;
    public float distanceThreshold;
    private bool objectInPlace;
    private bool hasDestroyed;

    // Define an event that gets triggered when the object is destroyed
    public delegate void KeyItemPlaced(GameObject sender);
    public event KeyItemPlaced OnKeyItemPlaced;

    private PickUpInteractor pickUpInteractor;
    private PuzzleKeyItem puzzleKeyItem;

    void Start()
    {
        pickUpInteractor = FindObjectOfType<PickUpInteractor>();
        puzzleKeyItem = GetComponent<PuzzleKeyItem>();
        objectInPlace = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Check distance between object and target location
        float dist = CalculateDistanceToTarget();
        
        // If this object is not in the player's hand
        if (!pickUpInteractor.IsHeld(gameObject)) {
            // If distance is close enough to target object and object was not yet in correct location
            // i.e. if obj placed in right location
            if (dist <= distanceThreshold) {
                targetObj.transform.localScale = new Vector3(0, 0, 0);
                puzzleKeyItem.HandleKeyItemPlaced();
                objectInPlace = true;
                targetObj.SetActive(false);
            }
        }
        else // if object is in player's hand
        {
            targetObj.SetActive(true);
            
            // If distance is too far from target object and object was just in correct location
            // i.e. if obj was removed from right location
            if (dist > distanceThreshold)  {
                puzzleKeyItem.HandleKeyItemRemoved();
                objectInPlace = false;
            }
            
        }
    }
    
    // Calculates the distance between this object and its target location
    private float CalculateDistanceToTarget()
    {
        Vector3 targetXZ = new Vector3(targetObj.transform.position.x, 0f, targetObj.transform.position.z);
        Vector3 objectXZ = new Vector3(transform.position.x, 0f, transform.position.z);
        float dist = Vector3.Distance(targetXZ, objectXZ);
        return dist;
    }
}