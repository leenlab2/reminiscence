using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintScreenshot : MonoBehaviour
{

    public RenderTexture renderTexture;
    [SerializeField] private GameObject PrefabPrintOut;
    private DetectPickUp _detectPickUp;
    
    // Start is called before the first frame update
    void Start()
    {
        _detectPickUp = FindObjectOfType<DetectPickUp>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintScreenshotPrintout()
    {
        Texture2D savedTexture = convertTo2DTexture();
       
        GameObject model = PrefabPrintOut.transform.GetChild(0).gameObject;
        GameObject paper = model.transform.GetChild(0).gameObject;
        GameObject image = paper.transform.GetChild(0).gameObject;
        
        Renderer renderer = image.GetComponent<Renderer>();
        Material material = new Material(Shader.Find("Specular"));
        material.SetTexture("_MainTex", savedTexture);
        renderer.material = material;
        GameObject printOut = Instantiate(PrefabPrintOut, new Vector3(0, 0, 0), Quaternion.identity);
        printOut.name = "Print Out"; 
        //_detectPickUp.DropObject();
        _detectPickUp.PickupObject(printOut.transform.GetChild(0).gameObject);
    }

    private Texture2D convertTo2DTexture()
    {
        Texture2D tex = new Texture2D(450, 256, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        return tex;
    }
}
