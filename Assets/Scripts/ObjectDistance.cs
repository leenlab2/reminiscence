using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDistance : MonoBehaviour
{
    public GameObject targetObj;
    public float distanceThreshold;
    private bool hasDestroyed;
    public bool objectSolved;

    // Define an event that gets triggered when the object is destroyed
    public delegate void KeyItemPlaced(GameObject sender);
    public event KeyItemPlaced OnKeyItemPlaced;

    float timeLeft = 3f;

    // Start is called before the first frame update
    void Start()
    {
        hasDestroyed = false;
        objectSolved = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasDestroyed) {
            Vector3 targetXZ = new Vector3(targetObj.transform.position.x, 0f, targetObj.transform.position.z);
            Vector3 objectXZ = new Vector3(transform.position.x, 0f, transform.position.z);
            float dist = Vector3.Distance(targetXZ, objectXZ);
            if (dist <= distanceThreshold) {
                Debug.Log("Correct: " + gameObject.name);
                targetObj.transform.localScale = new Vector3(0, 0, 0);
                OnKeyItemPlaced(gameObject);

                // Destroy(targetObj);
                hasDestroyed = true;
                objectSolved = true;
                var outline = gameObject.GetComponent<Outline>();
                outline.OutlineWidth = 5f;
            }
        }
        if (hasDestroyed) {
            timeLeft -= Time.deltaTime;
        }
        if ( timeLeft < 0 ) {
            var outline = gameObject.GetComponent<Outline>();
            outline.OutlineWidth = 0f;
        }
    }
}
