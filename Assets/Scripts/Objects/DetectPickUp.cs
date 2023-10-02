using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectPickUp : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] Transform holdArea;
    private GameObject heldObj;
    private Rigidbody heldObjRB;
    [SerializeField] Image crossHairs;
    [SerializeField] Sprite objectDetected;
    [SerializeField] Sprite noObjectDetected;
    //[SerializeField] public LayerMask layersToHit;


    [Header("Physics Parameters")]
    [SerializeField] private float pickupRange = 5.0f;
    [SerializeField] private float pickupForce = 150.0f;

// Update is called once per frame
void Update()
    {
    //Shoot out a ray every frame
    RaycastHit hit;
    NotDetected();
    
    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickupRange) 
        && (hit.transform.gameObject.tag == "LightObj"))
    {
            //WE ARE HITTING THE A MOVABLE OBJECT
        Detected();
        
        if(Input.GetMouseButtonDown(0))
        {
            if (heldObj == null)
            {
                PickupObject(hit.transform.gameObject);
                Debug.Log("Picked up Object");
            }
            else
            {
                DropObject();
            }
        }
    }
    
    if (heldObj != null)
    {
        MoveObject();
    }

}

    void MoveObject()
    {
        if (Vector3.Distance(heldObj.transform.position, holdArea.position) > 0.1f)
        {
            Vector3 moveDirection = (holdArea.position - heldObj.transform.position);
            heldObjRB.AddForce(moveDirection * pickupForce);
        }
    }

    void PickupObject(GameObject pickObj)
    {
        if (pickObj.GetComponent<Rigidbody>())
        {
            heldObjRB = pickObj.GetComponent<Rigidbody>();
            heldObjRB.useGravity = false;
            heldObjRB.drag = 10;
            heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;

            heldObjRB.transform.parent = holdArea;
            heldObj = pickObj;
        }
    }

    void DropObject()
    {
        heldObjRB.useGravity = true;
        heldObjRB.drag = 1;
        heldObjRB.constraints = RigidbodyConstraints.None;

        heldObjRB.transform.parent = null;
        heldObj = null;
    }

    void Detected()
    {
        crossHairs.sprite = objectDetected;
    }

    void NotDetected()
    {
        crossHairs.sprite = noObjectDetected;
    }
}
