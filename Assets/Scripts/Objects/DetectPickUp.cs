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
    [SerializeField] GameObject camera;
    private ObjectDistance listener = null;

    [Header("Physics Parameters")]
    [SerializeField] private float pickupRange = 5.0f;
    [SerializeField] private float pickupForce = 150.0f;

    private RaycastHit? currentHit = null;

    // Update is called once per frame
    void Update()
    {
        //Shoot out a ray every frame
        currentHit = null;
        RaycastHit hit;
        NotDetected();// Crosshairs function

        if(listener != null)
        {
            if (listener.objectSolved == true)
            {
                ToggleHoldObject();
                //listener = null;
            }
        }
    
        if (Physics.Raycast(camera.transform.position, camera.transform.TransformDirection(Vector3.forward), out hit, pickupRange) 
            && (hit.transform.gameObject.tag == "LightObj")) //Object must be tagged "LightObj" in order to be picked up
        {
            
            Detected();//Crosshairs function TODO: Fix crosshairs changing on listener == True objects
            currentHit = hit;
        }

        if (heldObj != null)
        {
            MoveObject();
            RotateObject();
        }

    }

    public void ToggleHoldObject()
    {
        if (heldObj == null && currentHit.HasValue)
        {
            PickupObject(currentHit.Value.transform.gameObject);
            Debug.Log(heldObj.name);
            Debug.Log("Picked up Object");
        }
        else
        {
            DropObject();
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

    void RotateObject()
    {
        //heldObj.transform.rotation = holdArea.transform.rotation - heldObj.transform.rotation;
        if(Quaternion.Angle(holdArea.transform.rotation, heldObj.transform.rotation) > 0.1f)
        {
            heldObj.transform.rotation = holdArea.transform.rotation * heldObj.transform.rotation;
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

            if (pickObj.GetComponent<ObjectDistance>()!=null)
            {
                listener = pickObj.GetComponent<ObjectDistance>();
            }

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
        listener = null;
    }

    void Detected()
    {
        //Debug.Log("I am looking at sth");
        crossHairs.sprite = objectDetected;
    }

    void NotDetected()
    {
        //Debug.Log("I am NOT looking at sth");
        crossHairs.sprite = noObjectDetected;
    }
}
