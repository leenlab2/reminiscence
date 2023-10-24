using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspection : MonoBehaviour
{
    public GameObject holdArea;

    private Camera cam;
    private Vector3 screenspaceCenter;
    private Vector3 localSidePosition;

    private void Awake()
    {
        cam = Camera.main;
        screenspaceCenter = new Vector3(Screen.width / 2, Screen.height / 2, 1.5f);
        localSidePosition = holdArea.transform.localPosition;
    }

    public void ToggleFocusObject(bool focus)
    {
        Vector3 newPosition;

        if (focus)
        {
            screenspaceCenter.z = CalcDistFromFace();
            Vector3 worldCamCenter = cam.ScreenToWorldPoint(screenspaceCenter);
            newPosition = cam.transform.InverseTransformPoint(worldCamCenter);
        } else
        {
            newPosition = localSidePosition;
        }

        holdArea.transform.localPosition = newPosition;
    }

    private float CalcDistFromFace()
    {
        Transform heldObj = holdArea.transform.GetChild(0);

        Renderer[] renderers = heldObj.GetComponentsInChildren<Renderer>();
        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; ++i)
        {
            bounds.Encapsulate(renderers[i].bounds.min);
            bounds.Encapsulate(renderers[i].bounds.max);
        }

        float largestDim = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        // calculate distance from face depending on object size such that it takes up most of the screen
        float distFromFace = largestDim / Mathf.Tan(cam.fieldOfView / 2 * Mathf.Deg2Rad);
        return distFromFace;
    }
}
