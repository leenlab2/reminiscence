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

    private string pickUp = "Left click to pick up";
    private string insertTape = "Left click to insert tape";
    private string removeTape = "Left click to remove tape";
    private string holdText = "Hold left to aim and click right to place. Press E to inspect.";
    private string inspectionText = "Use mouse to rotate object. Press E to exit inspection.";
    private string empty = "";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("InteractionCue");
        _interactText = GameObject.Find("Interact Text").GetComponent<TMP_Text>();
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
            _interactText.text = pickUp;
        } else if (type == InteractionCueType.InsertTape)
        {
            _interactText.text = insertTape;
        } else if (type == InteractionCueType.RemoveTape)
        {
            _interactText.text = removeTape;
        } else if (type == InteractionCueType.Hold)
        {
            _interactText.text = holdText;
        } else if (type == InteractionCueType.Inspection)
        {
            _interactText.text = inspectionText;
        }
    }
}
