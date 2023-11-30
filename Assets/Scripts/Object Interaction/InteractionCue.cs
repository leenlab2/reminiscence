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
    Branching,
    ExitMemory,
    EnterMemory,
    EnterTV,
    ExitTV,
}

public class InteractionCue : MonoBehaviour
{
    private TMP_Text _interactText;
    private TMP_Text _pickupText;
    private TMP_Text _TVText;
    private TMP_Text _dialogueText;

    // dialogue text
    public StringValue dialogueTextInfo;

    // xbox controls
    private string xboxEnterTV = "<sprite=0> Enter TV mode";
    private string xboxExitTV = "<sprite=0> Exit TV mode";
    private string xboxPickUp = "<sprite=1> Pickup";
    private string xboxInsertTape = "<sprite=1> Insert tape";
    private string xboxRemoveTape = "<sprite=1> Remove tape";
    // private string xboxPutDown = "<sprite=5> Inspect";
    private string xboxHoldText = "<sprite=1> Put down                        <sprite=3> Inspect";
    private string xboxInspectionText = "<sprite=4> Rotate                        <sprite=2> Exit";
    private string xboxBranching = "<sprite=4> Switch                         <sprite=1> Select";
    private string xboxExitMemoryText = "<sprite=2> Exit Memory";

    // keyboard/mouse controls
    private string kmEnterTV = "<sprite=12> Enter TV mode";
    private string kmExitTV = "<sprite=12> Exit TV mode";
    private string kmPickUp = "<sprite=16> Pickup";
    private string kmInsertTape = "<sprite=16> Insert tape";
    private string kmRemoveTape = "<sprite=16> Remove tape";
    // private string kmPutDown = "[E] Inspect";
    private string kmHoldText = "<sprite=16> Put down                        <sprite=15> Inspect";
    private string kmInspectionText = "<sprite=17> Rotate                        <sprite=15> Exit";
    private string kmBranching = "<sprite=14> Switch                         <sprite=16> Select";
    private string kmExitMemoryText = "<sprite=13> Exit Memory";

    private string empty = "";

    // input type
    private bool isController = true;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("InteractionCue");
        _interactText = GameObject.Find("Interact Text").GetComponent<TMP_Text>();
        _pickupText = GameObject.Find("Pickup Text").GetComponent<TMP_Text>();
        _TVText = GameObject.Find("TV Interaction Text").GetComponent<TMP_Text>();
        _dialogueText = GameObject.Find("Dialogue Text").GetComponent<TMP_Text>();

        if (isController)
        {
            _TVText.text = xboxEnterTV;
        }
        else
        {
            _TVText.text = kmEnterTV;
        }
    }

    public void ToggleController()
    {
        isController = !isController;
    }

    public void SetInteractionCue(InteractionCueType type)
    {
        if (type == InteractionCueType.Empty)
        {
            _pickupText.text = empty;
            _interactText.text = empty;
            _dialogueText.text = empty;

        } else if (type == InteractionCueType.Pickup)
        {
            if (isController)
            {
                _pickupText.text = xboxPickUp;
            } 
            else
            {
                _pickupText.text = kmPickUp;
            }
        } else if (type == InteractionCueType.InsertTape)
        {
            if (isController)
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
            if (isController)
            {
                _pickupText.text = xboxRemoveTape;
            } 
            else 
            {
                _pickupText.text = kmRemoveTape;
            }
        } else if (type == InteractionCueType.Hold)
        {
            //Debug.Log("xbox hold");
            if (isController)
            {
                _pickupText.text = "";
                _interactText.text = xboxHoldText;
            }
            else
            {
                _pickupText.text = "";
                _interactText.text = kmHoldText;
            }
            _dialogueText.text = empty;
        } else if (type == InteractionCueType.Inspection)
        {
            if (isController)
            {
                _interactText.text = xboxInspectionText;
            }
            else
            {
                _interactText.text = kmInspectionText;
            }
            _dialogueText.text = dialogueTextInfo.value;  //TODO: change with dynamic value
            _pickupText.text = empty;
        } else if (type == InteractionCueType.Branching)
        {
            if (isController)
            {
                _interactText.text = xboxBranching;
            }
            else
            {
                _interactText.text = kmBranching;
            }
            _dialogueText.text = empty;
            _pickupText.text = empty;
        } else if (type == InteractionCueType.ExitMemory)
        {
            InputManager inputManager = FindObjectOfType<InputManager>();
            if (isController)
            {
                if (inputManager.InTVMode())
                {
                    _TVText.text = xboxExitTV;
                } else
                {
                    _TVText.text = xboxEnterTV;
                }
            }
            else
            {
                if (inputManager.InTVMode())
                {
                    _TVText.text = kmExitTV;
                } else
                {
                    _TVText.text = kmEnterTV;
                }
            }
        } else if (type == InteractionCueType.EnterMemory)
        {
            if (isController)
            {
                _TVText.text = xboxExitMemoryText;
            }
            else
            {
                _TVText.text = kmExitMemoryText;
            }
        } else if (type == InteractionCueType.EnterTV)
        {
            if (isController)
            {
                _TVText.text = xboxEnterTV;
            }
            else
            {
                _TVText.text = kmEnterTV;
            }
        } else if (type == InteractionCueType.ExitTV)
        {
            if (isController)
            {
                _TVText.text = xboxExitTV;
            }
            else
            {
                _TVText.text = kmExitTV;
            }
        }
    }
}
