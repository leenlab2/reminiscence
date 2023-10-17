using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Tape Contents", fileName = "New Tape")]
public class TapeSO : ScriptableObject
{
    public VideoClip originalCorruptedVideoClip;
    public VideoClip branchACorruptedVideoClip;
    public VideoClip branchBCorruptedVideoClip;
    [SerializeField] private VideoClip branchASolutionVideoClip;
    [SerializeField] private VideoClip branchBSolutionVideoClip;
    
    [SerializeField] private int timeGlitchFixedInFixedTape;

    private bool tapeIsFixed = false;
    private ClipToPlay tapeSolutionBranch;
    
    public TapeSO(int timeGlitchFixedInFixedTape)
    {
        this.timeGlitchFixedInFixedTape = timeGlitchFixedInFixedTape;
    }

    public VideoClip GetVideoClip()
    {
        if (tapeIsFixed)
        {
            if (tapeSolutionBranch == ClipToPlay.BranchASolution)
            {
                return branchASolutionVideoClip;
            }
            else if (tapeSolutionBranch == ClipToPlay.BranchBSolution)
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

    public void SetTapeToFixed(ClipToPlay solutionBranch)
    {
        tapeIsFixed = true;
        tapeSolutionBranch = solutionBranch;
    }

}