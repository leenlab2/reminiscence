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
    EnterMemory
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
    private string xboxTV = "<sprite=7> TV mode";
    private string xboxPickUp = "<sprite=4> Pickup";
    private string xboxInsertTape = "<sprite=4> Insert tape";
    private string xboxRemoveTape = "<sprite=4> Remove tape";
    // private string xboxPutDown = "<sprite=5> Inspect";
    private string xboxHoldText = "<sprite=4> Put down                        <sprite=6> Inspect";
    private string xboxInspectionText = "<sprite=42> Rotate                        <sprite=5> Exit";
    private string xboxBranching = "<sprite=27><sprite=31> <sprite=25> Switch                         <sprite=4> Select";
    private string xboxExitMemoryText = "<sprite=5> Exit Memory";

    // keyboard/mouse controls
    private string kmTV = "[T] TV mode";
    private string kmPickUp = "[LeftClick] Pickup";
    private string kmInsertTape = "[LeftClick] Insert tape";
    private string kmRemoveTape = "[LeftClick] Remove tape";
    // private string kmPutDown = "[E] Inspect";
    private string kmHoldText = "[LeftClick] Put down                        [E] Inspect";
    private string kmInspectionText = "[Mouse] Rotate                        [E] Exit";
    private string kmBranching = "[A] / [D] Switch                         [LeftClick] Select";
    private string kmExitMemoryText = "[Tab] Exit Memory";

    private string empty = "";

    // input type
    [SerializeField] private bool isController = true;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("InteractionCue");
        _interactText = GameObject.Find("Interact Text").GetComponent<TMP_Text>();
        _pickupText = GameObject.Find("Pickup Text").GetComponent<TMP_Text>();
        _TVText = GameObject.Find("TV Interaction Text").GetComponent<TMP_Text>();
        _dialogueText = GameObject.Find("Dialogue Text").GetComponent<TMP_Text>();

        if (isController)
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
            Debug.Log("xbox hold");
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
            if (isController)
            {
                _TVText.text = xboxTV;
            }
            else
            {
                _TVText.text = kmTV;
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
        }
    }
}
