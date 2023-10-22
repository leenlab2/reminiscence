using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableDetector : MonoBehaviour
{
    // Inspector fields
    [SerializeField] private float maxPlayerReach = 5.0f;

    [Header("Crosshairs")]
    [SerializeField] private Image _crossHairDisplay;
    [SerializeField] private Sprite _defaultCrosshair;
    [SerializeField] private Sprite _objectDetected;

    // private fields
    private RaycastHit? _currentHit = null;
    private bool _crosshairOnTelevision = false;

    private void Update()
    {
        _currentHit = null;
        RaycastHit hit;
        _crosshairOnTelevision = false;
        NotDetected();

        // Define the ray we are using to detect objects
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(origin, direction, out hit, maxPlayerReach))
        {
            if (hit.transform.gameObject.tag == "LightObj")
            {
                Detected();
                _currentHit = hit;
            } else if (hit.transform.gameObject.tag == "TV" || hit.transform.parent?.name == "TV")
            {
                Detected();
                _currentHit = hit;
                _crosshairOnTelevision = true;
            }
        }
    }

    private void Detected()
    {
        _crossHairDisplay.sprite = _objectDetected;
    }

    private void NotDetected()
    {
        _crossHairDisplay.sprite = _defaultCrosshair;
    }

    public void InteractWithObject()
    {
        PickUpInteractor pickUpInteractor = GetComponent<PickUpInteractor>();
        if (!_currentHit.HasValue && pickUpInteractor.HeldObj == null) return;

        // Interact with TV
        if (_crosshairOnTelevision)
        {
            TapeManager tapeManager = FindObjectOfType<TapeManager>();

            if (pickUpInteractor.IsHeld("VHS_Tape"))
            {
                tapeManager.insertTape(pickUpInteractor.HeldObj);
            } else if (pickUpInteractor.HeldObj == null)
            {
                tapeManager.removeTape();
            }   
        } else // Pick up/Drop object
        {
            pickUpInteractor.ToggleHoldObject(_currentHit);
        }
    }
}
