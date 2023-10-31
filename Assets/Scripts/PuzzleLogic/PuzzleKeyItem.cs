using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Branch
{
    BranchA,
    BranchB,
    None
}

/// <summary>
/// Abstract superclass for branching items, and non branching key items
/// </summary>
public abstract class PuzzleKeyItem : MonoBehaviour
{
    public Outline outline; // points to Outline script
    
    public const float timeLengthOutline = 3f; // how long the outline should stay when object placed in right location
    public float timeLeft = timeLengthOutline; // amount of time left for the outline to stay.

    public PuzzleManagerNew puzzleManager;
    
    public void Start()
    {
        outline = GetComponent<Outline>();
        puzzleManager = GameObject.Find("Puzzle Manager").GetComponent<PuzzleManagerNew>();
    }

    // Update is called once per frame
    public void Update()
    {
        // Remove outline if already been outlined for 3 seconds
        timeLeft-= Time.deltaTime;
        if ( timeLeft < 0 ) { 
            outline.OutlineWidth = 0f;
        }
    }

    public abstract void HandleKeyItemPlaced();
}
