using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintScreenshot : MonoBehaviour
{

    public RenderTexture renderTexture;
    [SerializeField] private GameObject PrefabPrintOut;
    private PickUpInteractor _pickup;
    
    void Start()
    {
        _pickup = FindObjectOfType<PickUpInteractor>();
        
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
        GameObject printOut = Instantiate(PrefabPrintOut, new Vector3(5, 5, 5), Quaternion.identity);
        printOut.name = "Print Out";
        
        try // Drop any object player is holding so it can hold the print out
        {
            _pickup.DropObject();
        }
        catch (Exception e)
        {
            Debug.Log("Not holding any object");
        }
        _pickup.PickupObject(printOut.transform.GetChild(0).gameObject);
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
