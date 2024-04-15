using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles when a Non Branching Key Item is placed in the right location.
/// </summary>
public class PuzzleNonBranchingKeyItem : PuzzleKeyItem
{
    public bool appearsOnBothBranches;
    
    // Note: this takes in parent root object
    public static event Action<GameObject> OnKeyItemPlaced;
    public AudioSource bgmAudio = null;
    
    // Light in memory scene to brighten
    private Light memoryLight;

    void Start()
    {
        base.Start();
        //memoryLight = GameObject.Find("Memory Light").GetComponent<Light>();
        PuzzleManager.OnLevelChange += HandleLevelChange;
    }

    private void OnDestroy()
    {
        PuzzleManager.OnLevelChange -= HandleLevelChange;
    }

    public override void HandleCorrectPosition()
    {
        // Make memory scene brighter
        // memoryLight.intensity += 20;
        
        base.HandleCorrectPosition();

        Debug.Log("Key Item Placed:" + name);

        OnKeyItemPlaced?.Invoke(transform.parent.gameObject);

        CorrectPuzzleLogic();
    }

    protected override void CorrectPuzzleLogic()
    {
        outline.OutlineWidth = 5f;
        timeLeft = timeLengthOutline;
    }

    void HandleLevelChange(int newlevel)
    {
        Debug.Log($"Level changed to {newlevel}, which is on item {name} for lv {level}");
        if (newlevel != level && bgmAudio != null)
        {
            Debug.Log("Level changed, disabling " + transform.parent.name);
            bgmAudio.mute = true;
        }
    }
}
