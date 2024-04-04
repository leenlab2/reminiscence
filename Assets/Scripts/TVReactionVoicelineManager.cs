using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;


/* This script is a dirty fix, ideally those would be managed by animation
 * timelines.
 */
public class TVReactionVoicelineManager : MonoBehaviour
{
    public static TVReactionVoicelineManager instance;

    private TapeReactions _tapeReactions = null;
    private string _voiceline;

    [Header("Audio")]
    private AudioClip _dialogueAudio;
    [SerializeField] private AudioSource audioSource;

    [Header("Subtitles")]
    public TMP_Text subtitlesText;

    private Coroutine resetCoroutine;
    private Coroutine scrollCoroutine;

    private TapeManager tapeManager;

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

        tapeManager = FindObjectOfType<TapeManager>();
    }

    void Start()
    {
        _voiceline = "Nothing Yet"; //tODO: REPLACE WITH EMPTY
        VideoControls.dialoguePrompt += Play;
    }

    private void OnDestroy()
    {
        VideoControls.dialoguePrompt -= Play;
    }

    public void Play()
    {
        Debug.Log("Playing Dialogue: " + _voiceline);
        float timer = _dialogueAudio.length;

        TMP_Text textField = subtitlesText;

        ShowSubtitles(timer, textField);
        PlayVoicelineAudio();
    }

    public void Stop()
    {
        audioSource.Stop();
        subtitlesText.text = "";

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
    public void SetDialogue(string text, AudioClip dialogueAudio)
    {
        Debug.Log("Setting Dialogue for: " + text);
        _voiceline = text;
        _dialogueAudio = dialogueAudio;
        //VideoControls.dialoguePrompt += PlayDialogue;
    }

    public float ChangeReactionVoicelineForTape(ClipToPlay currentClip)
    {
        Debug.Log("Changing voiceline for tape");
        float timer = 0.0f;

        switch(currentClip)
        {
            case ClipToPlay.OriginalCorrupted:
                SetDialogue(_tapeReactions.startSubtitles, _tapeReactions.start);
                timer = 0.1f;
                break;

            case ClipToPlay.BranchACorrupted:
                SetDialogue(_tapeReactions.middleSubtitles, _tapeReactions.middle);
                timer = 0.8f;
                break;

            case ClipToPlay.BranchBCorrupted:
                SetDialogue(_tapeReactions.middleSubtitles, _tapeReactions.middle);
                timer = 0.8f;
                break;

            case ClipToPlay.BranchASolution:
                SetDialogue(_tapeReactions.endASubtitles, _tapeReactions.endA);
                timer = 0.8f;
                break;

            case ClipToPlay.BranchBSolution:
                SetDialogue(_tapeReactions.endBSubtitles, _tapeReactions.endB);
                timer = 0.8f;
                break;
        }

        return timer;
    }
    #endregion

    #region Play/Stop Audio
    public void PlayVoicelineAudio()
    {
        audioSource.clip = _dialogueAudio;
        audioSource.PlayOneShot(audioSource.clip, 1.0F);
    }
    #endregion

    #region Subtitles
    public void ShowSubtitles(float timer, TMP_Text uitext)
    {
        Debug.Log("Turning ON subtitles");

        uitext.text = _voiceline;
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

    private void Update()
    {
        if (tapeManager.televisionHasTape())
        {
            _tapeReactions = tapeManager.GetTapeReactionsInTV();
        }
    }
}

