using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspection : MonoBehaviour
{
    public GameObject holdArea;

    private Camera cam;
    private Vector3 screenspaceCenter;
    private Vector3 localSidePosition;

    private Bounds heldObjBounds;

    private void Awake()
    {
        cam = Camera.main;
        screenspaceCenter = new Vector3(Screen.width / 2, Screen.height / 2, 1.5f);
        localSidePosition = holdArea.transform.localPosition;
    }

    public void ToggleFocusObject(bool focus)
    {
        Vector3 newPosition;
        string newLayer;

        if (focus)
        {
            screenspaceCenter.z = CalcDistFromFace();
            Vector3 worldCamCenter = cam.ScreenToWorldPoint(screenspaceCenter);
            Vector3 offset = holdArea.transform.position - heldObjBounds.center;
            newPosition = cam.transform.InverseTransformPoint(worldCamCenter + offset);

            newLayer = "Inspection";
        } else
        {
            newPosition = localSidePosition;

            newLayer = "Default";
        }

        holdArea.transform.localPosition = newPosition;
        ChangeObjectLayer(holdArea.transform.GetChild(0), newLayer);
    }

    public void ChangeObjectLayer(Transform obj, string layerName)
    {
        obj.gameObject.layer = LayerMask.NameToLayer(layerName);

        foreach (Transform child in obj)
        {
            ChangeObjectLayer(child, layerName);
        }
    }

    private void GetObjectBounds()
    {
        Transform heldObj = holdArea.transform.GetChild(0);

        Collider[] colliders = heldObj.GetComponentsInChildren<Collider>();
        Bounds bounds = colliders[0].bounds;

        for (int i = 1; i < colliders.Length; ++i)
        {
            bounds.Encapsulate(colliders[i].bounds.min);
            bounds.Encapsulate(colliders[i].bounds.max);
        }

        heldObjBounds = bounds;
    }

    private float CalcDistFromFace()
    {
        GetObjectBounds();

        float largestDim = Mathf.Max(heldObjBounds.size.x, heldObjBounds.size.y, heldObjBounds.size.z);

        // calculate distance from face depending on object size such that it takes up most of the screen
        float distFromFace = largestDim / Mathf.Tan(cam.fieldOfView / 2 * Mathf.Deg2Rad);

        return distFromFace;
    }

    public void RotateObject(Vector2 rotationInput)
    {
        if (holdArea.transform.childCount == 0) return;

        GetObjectBounds();
        Transform heldObjectRotation = holdArea.transform;
        Vector3 objectCenter = heldObjBounds.center;
        heldObjectRotation.RotateAround(objectCenter, Vector3.up, rotationInput.x);
        heldObjectRotation.RotateAround(objectCenter, Vector3.right, rotationInput.y);
    }
}
