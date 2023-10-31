using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum InteractionCueType
{
    Empty,
    Pickup,
    InsertTape,
    RemoveTape,
    Hold,
    Inspection,

}

public class InteractionCue : MonoBehaviour
{
    private TMP_Text _interactText;
    private TMP_Text _pickupText;

    private string pickUp = "[LeftClick] Pickup";
    private string insertTape = "[LeftClick] insert tape";
    private string removeTape = "[LeftClick to remove";
    private string putDown = "[E] to inspect";
    private string holdText = "[Hold LeftClick + RightClick] Put down";
    private string inspectionText = "[Mouse] to rotate              [E] to exit";
    private string empty = "";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("InteractionCue");
        _interactText = GameObject.Find("Interact Text").GetComponent<TMP_Text>();
        _pickupText = GameObject.Find("Pickup Text").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInteractionCue(InteractionCueType type)
    {
        if (type == InteractionCueType.Empty)
        {
            _interactText.text = empty;
        } else if (type == InteractionCueType.Pickup)
        {
            _pickupText.text = pickUp;
        } else if (type == InteractionCueType.InsertTape)
        {
            _pickupText.text = insertTape;
            _interactText.text = empty;
        } else if (type == InteractionCueType.RemoveTape)
        {
            _pickupText.text = removeTape;
        } else if (type == InteractionCueType.Hold)
        {
            _pickupText.text = putDown;
            _interactText.text = holdText;
        } else if (type == InteractionCueType.Inspection)
        {
            _pickupText.text = empty;
            _interactText.text = inspectionText;
        }
    }
}
