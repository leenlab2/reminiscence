using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraSlider : MonoBehaviour
{
    [SerializeField] private float maxSliderAmount = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CameraSliderChange(float value)
    {
        float localValue = value * maxSliderAmount;
        float changeValue = localValue - 1.0f;
        InputManager inputManager = FindObjectOfType<InputManager>();
        inputManager.changeCameraSpeed(1.0f + (float) Math.Tan(changeValue * 0.5f));
    }
}
