using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EndingManager : MonoBehaviour
{
    public VideoClip EndingA;
    public VideoClip EndingB;
    public VideoClip Credits;

    private VideoPlayer _videoPlayer;
    private bool firstClipFinished = false;

    private void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();

        if (GameState.ending == Ending.EndingA)
        {
            _videoPlayer.clip = EndingA;
        }
        else
        {
            _videoPlayer.clip = EndingB;
        }

        _videoPlayer.Play();
    }

    private void Update()
    {
        if (!_videoPlayer.isPlaying && !firstClipFinished && _videoPlayer.time > 0)
        {
            firstClipFinished = true;
            _videoPlayer.clip = Credits;
            _videoPlayer.Play();
        }
        else if (!_videoPlayer.isPlaying && firstClipFinished && _videoPlayer.time > 0)
        {
            Debug.Log("Game finished");
            GameLoader.OnResetGame?.Invoke();
            StartCoroutine(GameLoader.LoadYourAsyncScene("Main Menu"));
        }
    }
}
