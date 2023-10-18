using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TapeManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private GameObject currentTapeInTv;
    private DetectPickUp _detectPickUp;
    
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
        _detectPickUp = FindObjectOfType<DetectPickUp>();
        
        videoPlayer.targetTexture.Release(); // ensure nothing is rendered on TV upon startup
    }

    private bool televisionHasTape()
    {
        if (currentTapeInTv != null)
        {
            return true;
        }
        return false;
    }

    public void insertTape(GameObject tapeGameObject)
    {
        if (televisionHasTape()) // if television already has tape in it, do nothing
        {
            print("HAD TAPE");
            return;
        }
        else // if television does not have tape, insert tape
        {
            // hide obj and put corresponding video clip on TV
            print("INSERTING TAPE");
            print(tapeGameObject);
            TapeSO tapeSO = tapeGameObject.GetComponent<TapeInformation>().TapeSO;
            videoPlayer.clip = tapeSO.GetVideoClip();
            videoPlayer.time = 0;
            videoPlayer.Play();
            videoPlayer.Pause();
            currentTapeInTv = tapeGameObject;
            tapeGameObject.active = false;
            _detectPickUp.DropObject();
        }
    }

    public void removeTape()
    {
        if (!televisionHasTape()) // if television does not have tape, do nothing
        {
            print("DO NOTHING");
            return;
        }
        else // if television has tape in it, remove it
        {
            print("REMOVING");
            // put obj back in hands of player and set video clip on TV to null
            // set clip on TV's player to null
            currentTapeInTv.active = true;
            videoPlayer.targetTexture.Release();
            _detectPickUp.PickupObject(currentTapeInTv);
            videoPlayer.clip = null;
            currentTapeInTv = null;

        }
    }
    
    public TapeSO GetCurrentTapeInTV()
    {
        return currentTapeInTv.GetComponent<TapeInformation>().TapeSO;
    }
}