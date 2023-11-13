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
    protected Outline outline; // points to Outline script

    public const float timeLengthOutline = 3f; // how long the outline should stay when object placed in right location
    protected float timeLeft = -1f; // amount of time left for the outline to stay.

    protected PuzzleManagerNew puzzleManager;
    
    // Objects that will appear in the memory scene when this branching item is placed
    [SerializeField] private List<GameObject> appearingObjects;

    public void Start()
    {
        
        outline = GetComponent<Outline>();
        puzzleManager = GameObject.Find("Puzzle Manager").GetComponent<PuzzleManagerNew>();
    }

    // Update is called once per frame
    public void Update()
    {
        if (timeLeft > 0)
        {
            // Remove outline if already been outlined for 3 seconds
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                outline.OutlineWidth = 0f;
            }
        }
    }

    public virtual void HandleCorrectPosition()
    {
        // Make corresponding objects appear to update memory scene
        MakeObjectsAppear();
        
        // Disable this script, prevent item from being interactable
        Destroy(GetComponent<PickupInteractable>());
        GetComponent<ObjectDistance>().enabled = false;
    }

    private void MakeObjectsAppear()
    {
        foreach (GameObject obj in appearingObjects)
        {
            obj.SetActive(true);
        }
    }

    protected abstract void CorrectPuzzleLogic();
}
