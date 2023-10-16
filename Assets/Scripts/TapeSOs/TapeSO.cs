using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Tape Contents", fileName = "New Tape")]
public class TapeSO : ScriptableObject
{
    [SerializeField] private VideoClip originalCorruptedVideoClip;
    [SerializeField] private VideoClip branchACorruptedVideoClip;
    [SerializeField] private VideoClip branchBCorruptedVideoClip;
    [SerializeField] private VideoClip branchASolutionVideoClip;
    [SerializeField] private VideoClip branchBSolutionVideoClip;
    
    [SerializeField] private int timeGlitchFixedInFixedTape;

    private bool tapeIsFixed = false;
    private string tapeSolutionBranch;
    
    public TapeSO(int timeGlitchFixedInFixedTape)
    {
        this.timeGlitchFixedInFixedTape = timeGlitchFixedInFixedTape;
    }

    public VideoClip GetVideoClip()
    {
        if (tapeIsFixed)
        {
            if (tapeSolutionBranch == "A")
            {
                return branchASolutionVideoClip;
            }
            else if (tapeSolutionBranch == "B")
            {
                return branchBSolutionVideoClip;
            }
        } 
        return originalCorruptedVideoClip;
        
    }
    
    public int GetTimeGlitchFixedInFixedTape()
    {
        return timeGlitchFixedInFixedTape;
    }

    public void SetTapeToFixed(string solutionBranch)
    {
        tapeIsFixed = true;
        tapeSolutionBranch = solutionBranch;
    }

}