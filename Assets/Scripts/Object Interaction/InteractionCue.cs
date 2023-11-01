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
    Branching
}

public class InteractionCue : MonoBehaviour
{
    private TMP_Text _interactText;
    private TMP_Text _pickupText;
    private TMP_Text _TVText;

    // xbox controls
    private string xboxTV = "<sprite=7> TV mode";
    private string xboxPickUp = "<sprite=4> Pickup";
    private string xboxInsertTape = "<sprite=4> Insert tape";
    private string xboxRemoveTape = "<sprite=4> Remove tape";
    // private string xboxPutDown = "<sprite=5> Inspect";
    private string xboxHoldText = "Hold <sprite=14> + <sprite=4> Put down                        <sprite=5> Inspect";
    private string xboxInspectionText = "<sprite=42> Rotate                        <sprite=5> Exit";
    private string xboxBranching = "<sprite=27><sprite=31> <sprite=25> Switch                         <sprite=4> Select";

    // keyboard/mouse controls
    private string kmTV = "[T] TV mode";
    private string kmPickUp = "[LeftClick] Pickup";
    private string kmInsertTape = "[LeftClick] Insert tape";
    private string kmRemoveTape = "[LeftClick] Remove tape";
    // private string kmPutDown = "[E] Inspect";
    private string kmHoldText = "Hold [LeftClick] + [RightClick] Put down                        [E] Inspect";
    private string kmInspectionText = "[Mouse] Rotate                        [E] Exit";
    private string kmBranching = "[A] / [D] Switch                         [LeftClick] Select";

    private string empty = "";

    // 0: xbox controller, 1: keyboard/mouse, 2: ps controller
    private int input = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("InteractionCue");
        _interactText = GameObject.Find("Interact Text").GetComponent<TMP_Text>();
        _pickupText = GameObject.Find("Pickup Text").GetComponent<TMP_Text>();
        _TVText = GameObject.Find("TV Interaction Text").GetComponent<TMP_Text>();

        if (input == 0)
        {
            _TVText.text = xboxTV;
        }
        else
        {
            _TVText.text = kmTV;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInteractionCue(InteractionCueType type)
    {
        if (type == InteractionCueType.Empty)
        {
            _pickupText.text = empty;
            _interactText.text = empty;
        } else if (type == InteractionCueType.Pickup)
        {
            if (input == 0)
            {
                _pickupText.text = xboxPickUp;
            } 
            else
            {
                _pickupText.text = kmPickUp;
            }
        } else if (type == InteractionCueType.InsertTape)
        {
            if (input == 0)
            {
                _pickupText.text = xboxInsertTape;
            } 
            else 
            {
                _pickupText.text = kmInsertTape;
            }
            _interactText.text = empty;
        } else if (type == InteractionCueType.RemoveTape)
        {
            if (input == 0)
            {
                _pickupText.text = xboxRemoveTape;
            } 
            else 
            {
                _pickupText.text = kmRemoveTape;
            }
        } else if (type == InteractionCueType.Hold)
        {
            Debug.Log("xbox hold");
            if (input == 0)
            {
                _pickupText.text = "";
                _interactText.text = xboxHoldText;
            }
            else
            {
                _pickupText.text = "";
                _interactText.text = kmHoldText;
            }
        } else if (type == InteractionCueType.Inspection)
        {
            if (input == 0)
            {
                _interactText.text = xboxInspectionText;
            }
            else
            {
                _interactText.text = kmInspectionText;
            }
            _pickupText.text = empty;
        }  else if (type == InteractionCueType.Branching)
        {
            if (input == 0)
            {
                Debug.Log("xbox branching");
                _interactText.text = xboxBranching;
            }
            else
            {
                _interactText.text = kmBranching;
            }
            _pickupText.text = empty;
        }
    }
}