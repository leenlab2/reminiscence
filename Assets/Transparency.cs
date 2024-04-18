using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Transparency : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        // slowly decrease the alpha value of the image
        image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + 0.001f);
    }
}
