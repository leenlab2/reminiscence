using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constantly calculates distance between this object and its target object.
/// If object is in correct location, trigger PuzzleItem <KeyItemPlaced> event.
/// </summary>
public class ObjectDistance : MonoBehaviour
{
    public GameObject targetObj;
    public float distanceThreshold;
    public bool isOnBothBranches = false;
    
    private PickUpInteractor pickUpInteractor;
    private PuzzleKeyItem puzzleKeyItem;
    
    protected void Start()
    {
        pickUpInteractor = FindObjectOfType<PickUpInteractor>();
        puzzleKeyItem = GetComponent<PuzzleKeyItem>();
    }

    protected void Update()
    {
        // Check distance between object and target location
        float dist = CalculateDistanceToTarget();
        
        // If this object is not in the player's hand
        if (!pickUpInteractor.IsHeld(gameObject)) {
            
            // If object is placed in right location
            if (dist <= distanceThreshold) {
                Debug.Log("HERE");
                targetObj.SetActive(false);
                puzzleKeyItem.HandleCorrectPosition();
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
    
    // Override this virtual method on subclass ObjectDistanceItemOnBothBranches
    public virtual void SwitchPuzzleTarget(Branch branch) {}
}