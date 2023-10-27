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

    public bool tapeIsFixed = false;
    public ClipToPlay tapeSolutionBranch;
    public ClipToPlay clipToPlay;

    public int level;
    
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
        else if (clipToPlay == ClipToPlay.BranchACorrupted)
        {
            return branchACorruptedVideoClip;
        }
        else if (clipToPlay == ClipToPlay.BranchBCorrupted)
        {
            return branchBCorruptedVideoClip;
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