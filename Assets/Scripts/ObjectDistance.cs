using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDistance : MonoBehaviour
{
    public GameObject targetObj;
    public float distanceThreshold;
    private bool hasDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        hasDestroyed = false;
    }

    // Update is called once per frame
    void Update()
    {
        while (!hasDestroyed) {
            Vector3 targetXZ = new Vector3(targetObj.transform.position.x, 0f, targetObj.transform.position.z);
            Vector3 objectXZ = new Vector3(transform.position.x, 0f, transform.position.z);
            float dist = Vector3.Distance(targetXZ, objectXZ);
            if (dist <= distanceThreshold) {
                Debug.Log("Correct!");
                Destroy(targetObj);
                hasDestroyed = true;
            }
        }
    }
}
