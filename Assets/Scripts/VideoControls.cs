using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoControls : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
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
        videoPlayer.time = videoPlayer.time + 3;
    }
    
    public void Replay()
    {
        print("Replay!");
        videoPlayer.time = 0;
        videoPlayer.Play();
    }
}
