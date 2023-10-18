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
        if (heldObjRB.name == "PosterA" || heldObjRB.name == "PosterB") {
            RaycastHit dropHit;
            if (Physics.Raycast(camera.transform.position, camera.transform.TransformDirection(Vector3.forward), out dropHit, pickupRange)) {
                Debug.Log("ray cast hit: " + dropHit.transform.gameObject.name);
                if (dropHit.transform.gameObject.name == "PosterRange") {

                    int branchState = 0;

                    Vector3 anchorPos = new Vector3(GameObject.Find("PosterAnchor").transform.position.x, GameObject.Find("PosterAnchor").transform.position.y, GameObject.Find("PosterAnchor").transform.position.z);
                    Vector3 aPos = new Vector3(GameObject.Find("PosterA").transform.position.x, GameObject.Find("PosterA").transform.position.y, GameObject.Find("PosterA").transform.position.z);
                    Vector3 bPos = new Vector3(GameObject.Find("PosterB").transform.position.x, GameObject.Find("PosterB").transform.position.y, GameObject.Find("PosterB").transform.position.z);

                    if (Vector3.Distance(anchorPos, aPos) < 1.0f) {
                        // poster A already placed
                        Debug.Log("poster A already placed");
                        branchState = 1;
                    } else if (Vector3.Distance(anchorPos, bPos) < 1.0f) {
                        // poster B already placed
                        Debug.Log("poster B already placed");
                        branchState = 2;
                    }

                    heldObjRB.transform.parent = null;
                    heldObj = null;
                    listener = null;
                    heldObjRB.transform.position = GameObject.Find("PosterAnchor").transform.position;
                    heldObjRB.transform.rotation = Quaternion.Euler(0, 90, 0);
                    heldObjRB.transform.localScale = new Vector3(0.05236547f, 0.05236546f, 0.05236547f);

                    if (branchState == 1) {
                        // poster A already placed
                        // PickupObject(GameObject.Find("PosterA"));
                        // Debug.Log("Picked up poster a");
                        GameObject posterA = GameObject.Find("PosterA");
                        posterA.transform.position = new Vector3(-5.92f, 0.11f, 13.95f);
                        posterA.transform.rotation = Quaternion.Euler(0, 90, 90);
                    } else if (branchState == 2) {
                        // poster B already placed
                        // PickupObject(GameObject.Find("PosterB"));
                        // Debug.Log("Picked up poster b");
                        GameObject posterB = GameObject.Find("PosterB");
                        posterB.transform.position = new Vector3(-5.92f, 0.11f, 13.95f);
                        posterB.transform.rotation = Quaternion.Euler(0, 90, 90);
                    }
                    return;
                }
            }
        }
        heldObjRB.useGravity = true;
        heldObjRB.drag = 1;
        heldObjRB.constraints = RigidbodyConstraints.None;

        heldObjRB.transform.parent = null;
        heldObj = null;
        listener = null;
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
