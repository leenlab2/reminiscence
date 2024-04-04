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
    public static DialogueManager instance;

    [Header("Info on Held Item")]
    private PickupInteractable _pickupInteractable;
    private string _inspectionObjectText;

    public StringValue textInfo;
    public AudioClipScriptableObject audioInfo;

    [Header("Audio")]
    public AudioClip dialogueAudio;
    [SerializeField] private AudioSource audioSource;

    [Header("Subtitles")]
    public TMP_Text subtitlesText;
    public TMP_Text reactionsText;

    private Coroutine resetCoroutine;
    private Coroutine scrollCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        _inspectionObjectText = "Nothing Yet"; //tODO: REPLACE WITH EMPTY
        VideoControls.dialoguePrompt += Play;
        PickUpInteractor.OnObjectPlace += HandlePlace;
    }

    private void OnDestroy()
    {
        VideoControls.dialoguePrompt -= Play;
        PickUpInteractor.OnObjectPlace -= HandlePlace;
    }
   
    void HandlePlace(GameObject obj) 
    {
        Stop();

        _pickupInteractable = null;
        _inspectionObjectText = "";
    }
    
    public void Play()
    {
        Debug.Log("Playing Dialogue: " + _inspectionObjectText);
        float timer = audioSource.clip.length;

        TMP_Text textField;
        if (InputManager.instance.InTVMode())
        {
            textField = reactionsText;
        } else
        {
            textField = subtitlesText;
        }

        ShowSubtitles(timer, textField);
        PlayVoicelineAudio();
    }

    public void Stop()
    {
        audioSource.Stop();
        subtitlesText.text = "";
        reactionsText.text = "";

        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }
        if (scrollCoroutine != null)
        {
            StopCoroutine(scrollCoroutine);
            scrollCoroutine = null;
        }
    }

    #region Set Dialogue + Subtitles
    public void SetDialogue(GameObject obj)
    {
        Debug.Log("Setting Dialogue for: " + obj.name);
        _pickupInteractable = obj.GetComponent<PickupInteractable>();
        _inspectionObjectText = _pickupInteractable.inspectionObjectText;
        audioSource.clip = _pickupInteractable.dialogueAudio;
    }

    public void SetDialogue(string text, AudioClip dialogueAudio)
    {
        Debug.Log("Setting Dialogue for: " + text);
        _inspectionObjectText = text;
        audioSource.clip = dialogueAudio;
        //VideoControls.dialoguePrompt += PlayDialogue;
    }
    #endregion

    #region Play/Stop Audio
    public void PlayVoicelineAudio()
    {
        audioSource.PlayOneShot(audioSource.clip, 1.0F);
    }
    #endregion

    #region Subtitles
    public void ShowSubtitles(float timer, TMP_Text uitext)
    {
        Debug.Log("Turning ON subtitles");

        uitext.text = _inspectionObjectText;
        uitext.ForceMeshUpdate();
        uitext.pageToDisplay = 1;

        // if the text spans multiple overflow pages
        if (uitext.textInfo.pageCount > 1)
        {
            scrollCoroutine = StartCoroutine(ScrollSubtitles(uitext, timer));
        }
        
        resetCoroutine = StartCoroutine(ResetSubtitles(uitext, timer));
    }

    public IEnumerator ScrollSubtitles(TMP_Text uitext, float timer)
    {
        int pages = uitext.textInfo.pageCount;
        float timePerPage = timer / pages;
        Debug.Log(timer);
        Debug.Log("Time per page: " + timePerPage);

        for (int i = 0; i < pages; i++)
        {
            Debug.Log("Scrolling to page: " + uitext.pageToDisplay);
            
            yield return new WaitForSeconds(timePerPage);
            uitext.pageToDisplay = i + 2;

        }
    }

    public IEnumerator ResetSubtitles(TMP_Text uitext, float timer)
    {
        yield return new WaitForSeconds(timer);
        uitext.text = "";
        Debug.Log("Turning off subtitles");
    }
    #endregion
}
