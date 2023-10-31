using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
// Subclass of ObjectDistanceNew class, placed on an object if the object appears in both branches
/// </summary>
public class ObjectDistanceItemOnBothBranches : ObjectDistance
{
    public GameObject targetObjA; // target object if this object appears on Branch A
    public GameObject targetObjB; // target object if this object also appears on Branch B
    
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        isOnBothBranches = true;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
    
    public override void SwitchPuzzleTarget(Branch branch)
    {
        if (branch == Branch.BranchA)
        {
            targetObj = targetObjA;
        }
        else if (branch == Branch.BranchB)
        {
            targetObj = targetObjB;
        }
    }
    
}
