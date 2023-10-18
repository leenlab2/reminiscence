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

    [Header("Physics Parameters")]
    [SerializeField] private float pickupRange = 5.0f;
    [SerializeField] private float pickupForce = 150.0f;

    private RaycastHit? currentHit = null;
    
    private bool crosshairOnTelevision = false;
    private TapeManager tapeManager;
    
    //Save State
    private Quaternion rotationReset;
    private Vector3 localScale;
    private Transform parent;

    //New State
    //private Vector3 newScale = new Vector3(0.5F, 0.5F, 0.5F);

    void Start()
    {
        rotationReset = holdArea.transform.rotation;
        tapeManager = FindObjectOfType<TapeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Shoot out a ray every frame
        currentHit = null;
        RaycastHit hit;
        NotDetected();// Crosshairs function
    
        if (Physics.Raycast(camera.transform.position, camera.transform.TransformDirection(Vector3.forward), out hit, pickupRange) 
            && (hit.transform.gameObject.tag == "LightObj")) //Object must be tagged "LightObj" in order to be picked up
        {
            
            Detected();//Crosshairs function TODO: Fix crosshairs changing on listener == True objects
            currentHit = hit;
        }
        
        crosshairOnTelevision = false; // set to false before checking if crosshair on television
        if (Physics.Raycast(camera.transform.position, camera.transform.TransformDirection(Vector3.forward), out hit, pickupRange) 
            && (hit.transform.name == "TV" || hit.transform.parent?.name == "TV")) //If holding a tape and clicked TV
        {
            crosshairOnTelevision = true;
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
            ResetHoldArea();

            PickupObject(currentHit.Value.transform.gameObject);
        }
        else if (crosshairOnTelevision){ // if clicked on TV
            if (heldObj != null && heldObj.name == "VHS_Tape") // if holding VHS tape, insert tape into TV
            {
                tapeManager.insertTape(heldObj);
                
            }
            else if(heldObj == null) // if holding nothing, remove tape from TV if any
            {
                tapeManager.removeTape();
            }
        }
        else
        {
            DropObject();
            ResetHoldArea();
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
        heldObjRB.isKinematic = false;
        if (Quaternion.Angle(holdArea.transform.rotation, heldObj.transform.rotation) > 0.1f)
        {
            heldObj.transform.rotation = holdArea.transform.rotation * heldObj.transform.rotation;
        }
    }
 

    public void PickupObject(GameObject pickObj)
    {
        if (pickObj.GetComponent<Rigidbody>())
        {
            parent = pickObj.transform.parent;

            heldObjRB = pickObj.GetComponent<Rigidbody>();
            heldObjRB.useGravity = false;
            heldObjRB.drag = 10;
            heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;
            heldObjRB.isKinematic = true;


            MakeObjSmall(pickObj);
            pickObj.transform.rotation = holdArea.transform.rotation;
            heldObjRB.transform.parent = holdArea;
            heldObj = pickObj;
            pickObj.transform.position = holdArea.transform.position;
            //MakeObjSmall(pickObj);
        }
    }

    public void DropObject()
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
                        posterA.transform.position = new Vector3(-11.57f, 2.07f, 13.95f);
                        posterA.transform.rotation = Quaternion.Euler(0, 90, 90);
                    } else if (branchState == 2) {
                        // poster B already placed
                        // PickupObject(GameObject.Find("PosterB"));
                        // Debug.Log("Picked up poster b");
                        GameObject posterB = GameObject.Find("PosterB");
                        posterB.transform.position = new Vector3(-11.57f, 2.07f, 13.95f);
                        posterB.transform.rotation = Quaternion.Euler(0, 90, 90);
                    }
                    return;
                }
            }
        }
        heldObjRB.isKinematic = false;
        heldObjRB.useGravity = true;
        heldObjRB.drag = 1;
        heldObjRB.constraints = RigidbodyConstraints.None;

        //heldObjRB.transform.parent = null;
        heldObjRB.transform.parent = parent;
        listener = null;
        MakeObjBig();
        heldObj = null;
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

    void ResetHoldArea()
    {
        holdArea.transform.rotation = rotationReset;
    }

    void MakeObjBig()
    {
        heldObj.transform.localScale = localScale;
    }
    void MakeObjSmall(GameObject pickObj)
    {
        localScale = pickObj.transform.localScale;
        Debug.Log(localScale);
        pickObj.transform.localScale = pickObj.transform.localScale / 2;  //newScale;
    }

}
