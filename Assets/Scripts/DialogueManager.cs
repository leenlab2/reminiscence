using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    private TMP_Text _interactText;
    private InteractionCue _interactionCue;
    private PickupInteractable _pickupInteractable;
    private string _inspectionObjectText;

    public StringValue objectTextInfo;
    public AudioClipScriptableObject objectAudioInfo;
    
    public AudioClip dialogueAudio;

    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        _inspectionObjectText = "Nothing Yet"; //tODO: REPLACE WITH EMPTY

        _interactText = GameObject.Find("Interact Text").GetComponent<TMP_Text>();
        _interactionCue = GameObject.Find("InteractionCue").GetComponent<InteractionCue>();
        audio = GameObject.Find("AudioDialogue").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setDialogue(GameObject obj)
    {
        _pickupInteractable = obj.GetComponent<PickupInteractable>();
        _inspectionObjectText = _pickupInteractable.inspectionObjectText;

        objectAudioInfo.SetAudioClip(_pickupInteractable.dialogueAudio);
        objectTextInfo.value = _inspectionObjectText;
    }

    public void setDialogueText(string text)
    {
        _inspectionObjectText = text;

        //objectAudioInfo.SetAudioClip(_pickupInteractable.dialogueAudio);
        objectTextInfo.value = _inspectionObjectText;
    }

    public void playDialogue()
    {
        //Dialogue Audio
        audio.clip = objectAudioInfo.GetSoundClip();
        audio.PlayOneShot(audio.clip, 1.0F);

    }

    public void playDialogueSubtitles()
    {
        _interactionCue.SetInteractionCue(InteractionCueType.SubtitlesOn);
    }

    public void stopDialogue()
    {
        audio.Stop();
    }

    public void playBranchingDialogue()
    {
        //Dialogue Audio
        audio.clip = objectAudioInfo.GetSoundClip();
        audio.PlayOneShot(audio.clip, 1.0F);

        //Show subtitles
        _interactionCue.SetInteractionCue(InteractionCueType.Branching);

        //Test connection
        Debug.Log("Doing the branching dialogue thing");
    }
}
