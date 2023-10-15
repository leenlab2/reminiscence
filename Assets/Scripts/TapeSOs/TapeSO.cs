using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Tape Contents", fileName = "New Tape")]
public class TapeSO : ScriptableObject
{
    [SerializeField] private VideoClip corruptedVideoClip;
    [SerializeField] private VideoClip fixedVideoClip;
    
    [SerializeField] private int timeGlitchFixedInFixedTape;

    private bool tapeIsFixed = false;

    public VideoClip GetVideoClip()
    {
        if (tapeIsFixed)
        {
            return fixedVideoClip;
        }
        else
        {
            return corruptedVideoClip;
        }
    }
    
    public int GetTimeGlitchFixedInFixedTape()
    {
        return timeGlitchFixedInFixedTape;
    }

    public void SetTapeToFixed()
    {
        tapeIsFixed = true;
    }

}