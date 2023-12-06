using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;


public class DialogueManager : MonoBehaviour
{
    private TMP_Text _interactText;
    private InteractionCue _interactionCue;
    private PickupInteractable _pickupInteractable;
    private string _inspectionObjectText;

    public StringValue objectTextInfo;
    public AudioClipScriptableObject objectAudioInfo;

    //Reactions
    public StringValue reactionTextInfo;
    public AudioClipScriptableObject reactionAudioInfo;
    
    public AudioClip dialogueAudio;

    private AudioSource audio;
    private AudioSource audioReaction;

    // Start is called before the first frame update
    void Start()
    {
        _inspectionObjectText = "Nothing Yet"; //tODO: REPLACE WITH EMPTY

        _interactText = GameObject.Find("Interact Text").GetComponent<TMP_Text>();
        _interactionCue = GameObject.Find("InteractionCue").GetComponent<InteractionCue>();
        audio = GameObject.Find("AudioDialogue").GetComponent<AudioSource>();
        audioReaction = GameObject.Find("AudioReaction").GetComponent<AudioSource>();

        VideoControls.dialoguePrompt += PlayDialogue;

    }

    private void OnDestroy()
    {
        VideoControls.dialoguePrompt -= PlayDialogue;
    }
   
    void PlayDialogue()
    {

        float timer = playReaction();
        if (timer > 10F) { timer = 10F; } //TAKE THIS OUT ONCE CLIPS HAVE BEEN TRIMMED
        playDialogueSubtitles(timer);
        Invoke(stopReaction, timer);
    }

    #region Set Dialogue + Subtitles
    public void setDialogue(GameObject obj)
    {

        _pickupInteractable = obj.GetComponent<PickupInteractable>();
        _inspectionObjectText = _pickupInteractable.inspectionObjectText;

        objectAudioInfo.SetAudioClip(_pickupInteractable.dialogueAudio);
        objectTextInfo.value = _inspectionObjectText;
    }

    /*public void SetDialogueNoObject(string text, AudioClip dialogueAudio)
    {
        reactionTextInfo.value = text;
        reactionAudioInfo.SetAudioClip(dialogueAudio);
    }*/

    #region Set Individual
    public void setDialogueText(string text)
    {
        _inspectionObjectText = text;
        objectTextInfo.value = _inspectionObjectText;
    }

    public void setDialogueAudio(AudioClip dialogueAudio)
    {
        objectAudioInfo.SetAudioClip(dialogueAudio);
    }
    #endregion
    #endregion

    #region Play/Stop Audio
    public void playDialogue()
    {
        //Dialogue Audio
        audio.clip = objectAudioInfo.GetSoundClip();
        audio.PlayOneShot(audio.clip, 1.0F);

    }

    public void stopDialogue()
    {
        audio.Stop();
    }
    #endregion



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

    #region Reactions
    public void SetDialogueNoObject(string text, AudioClip dialogueAudio)
    {
        reactionTextInfo.value = text;
        reactionAudioInfo.SetAudioClip(dialogueAudio);
        //VideoControls.dialoguePrompt += PlayDialogue;
    }
    public float playReaction()
    {
        //Dialogue Audio
        audioReaction.clip = reactionAudioInfo.GetSoundClip();
        audioReaction.PlayOneShot(audioReaction.clip, 1.0F);
        return audioReaction.clip.length;

    }

    public void stopReaction()
    {
        audioReaction.Stop();
    }

    #region Subtitles
    public void playDialogueSubtitles(float timer)
    {
        //_interactionCue.SetInteractionCue(InteractionCueType.SubtitlesOn); 
        //_interactionCue.SetInteractionCue(InteractionCueType.SubtitlesOff);

        showSubtitles();
        Invoke("hideSubtitles", timer);

    }

    #region Subtitle Helpers
    public void showSubtitles()
    {
        _interactionCue.SetInteractionCue(InteractionCueType.SubtitlesOn);
        Debug.Log("Turning ON subtitles");

    }

    public void hideSubtitles()
    {
        _interactionCue.SetInteractionCue(InteractionCueType.SubtitlesOff);
        Debug.Log("Turning off subtitles");
        
    }
    #endregion
    #endregion
    #endregion
}
