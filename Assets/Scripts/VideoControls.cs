using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoControls : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public Image _progressBarImage;
    public GameObject _televisionCanvas;


    void Start()
    {
        videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
        _progressBarImage.fillAmount = 0;
    }

    public void PauseOrPlay()
    {
        if (videoPlayer.isPlaying)
        {
            Pause();
        }
        else
        {
            print("Play!");
            videoPlayer.Play();
        }
    }

    public void Pause()
    {
        print("Pause!");
        videoPlayer.Pause();
    }
    
    public void Rewind()
    {
        print("Rewind!");
        videoPlayer.time = videoPlayer.time - 3;
    }

    public void FastForward()
    {
        print("FastForward!");
        videoPlayer.time = videoPlayer.time + 5;
    }
    
    public void Replay()
    {
        print("Replay!");
        videoPlayer.time = 0;
        videoPlayer.Play();
    }

    void Update()
    {
        float progressPercentage = (float) (videoPlayer.time / videoPlayer.length); 
        _progressBarImage.fillAmount = progressPercentage;
    }
}
