using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoControls : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    private bool _rewind = false;
    void Start()
    {
        videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
    }

    public void Pause()
    {
        print("Pause!");
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
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
        //videoPlayer.playbackSpeed = 5.0f;
    }
    
    public void Replay()
    {
        print("Replay!");
        videoPlayer.time = 0;
        videoPlayer.Play();
    }
}
