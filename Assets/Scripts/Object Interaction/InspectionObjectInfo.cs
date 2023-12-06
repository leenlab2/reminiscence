using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectionObjectInfo : MonoBehaviour
{
    public StringValue objectTextInfo;
    public string inspectionObjectText;

    // Start is called before the first frame update
    private void Start()
    {
        objectTextInfo.value = inspectionObjectText;
        
    }

}
