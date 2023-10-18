using System.Collections;   
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    /// <summary>
    /// 0: tut level; 1: first level; 2: second level
    /// </summary>
    private int level;

    /// <summary>
    /// 0: no branch; 1: branch A; 2: branch B
    /// </summary>
    private int branch;

    // private List<ObjectDistance> tutKeyItemsA;
    // private List<ObjectDistance> tutKeyItemsB;
    private List<ObjectDistance> firstKeyItemsA;
    private List<ObjectDistance> firstKeyItemsB;
    // private List<ObjectDistance> secondKeyItemsA;
    // private List<ObjectDistance> secondKeyItemsB;

    // [SerializeField] private List<GameObject> tutKeyGameObjsA;
    // [SerializeField] private List<GameObject> tutKeyGameObjsB;
    [SerializeField] private List<GameObject> firstKeyGameObjsA;
    [SerializeField] private List<GameObject> firstKeyGameObjsB;
    // [SerializeField] private List<GameObject> secondKeyGameObjsA;
    // [SerializeField] private List<GameObject> secondKeyGameObjsB;

    // [SerializeField] private GameObject tutBranchingObjA;
    // [SerializeField] private GameObject tutBranchingObjB;
    [SerializeField] private GameObject firstBranchingObjA;
    [SerializeField] private GameObject firstBranchingObjB;
    // [SerializeField] private GameObject secondBranchingObjA;
    // [SerializeField] private GameObject secondBranchingObjB;

    public delegate void PuzzleComplete();
    public event PuzzleComplete OnPuzzleComplete;

    private VideoControls _videoControls;

    void Start()
    {
        _videoControls = FindObjectOfType<VideoControls>();
        level = 1;
        branch = 0;
    }

    private void Awake()
    {
        // keyItems = FindObjectsOfType<ObjectDistance>().ToList();
        // tutBranchingObjA.GetComponent<ObjectDistance>().OnKeyItemPlaced += HandleKeyItemPlaced;
        // tutBranchingObjB.GetComponent<ObjectDistance>().OnKeyItemPlaced += HandleKeyItemPlaced;
        firstBranchingObjA.GetComponent<ObjectDistance>().OnKeyItemPlaced += HandleKeyItemPlaced;
        firstBranchingObjB.GetComponent<ObjectDistance>().OnKeyItemPlaced += HandleKeyItemPlaced;
        // secondBranchingObjA.GetComponent<ObjectDistance>().OnKeyItemPlaced += HandleKeyItemPlaced;
        // secondBranchingObjB.GetComponent<ObjectDistance>().OnKeyItemPlaced += HandleKeyItemPlaced;

        // foreach (var obj in tutKeyGameObjsA)
        // {
        //     ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
        //     objDist.OnKeyItemPlaced += HandleKeyItemPlaced;
        //     tutKeyItemsA.Add(objDist);
        // }

        // foreach (var obj in tutKeyGameObjsB)
        // {
        //     ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
        //     objDist.OnKeyItemPlaced += HandleKeyItemPlaced;
        //     tutKeyItemsB.Add(objDist);
        // }

        Debug.Log("puzzle manager");


        foreach (var obj in firstKeyGameObjsA)
        {
            Debug.Log(obj.name);
            ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
            objDist.OnKeyItemPlaced += HandleKeyItemPlaced;
            // firstKeyItemsA.Add(objDist);
        }

        foreach (var obj in firstKeyGameObjsB)
        {
            ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
            objDist.OnKeyItemPlaced += HandleKeyItemPlaced;
            // firstKeyItemsB.Add(objDist);
        }

        // foreach (var obj in secondKeyGameObjsA)
        // {
        //     ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
        //     objDist.OnKeyItemPlaced += HandleKeyItemPlaced;
        //     secondKeyItemsA.Add(objDist);
        // }

        // foreach (var obj in secondKeyGameObjsB)
        // {
        //     ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
        //     objDist.OnKeyItemPlaced += HandleKeyItemPlaced;
        //     secondKeyItemsB.Add(objDist);
        // }
    }

    private void HandleKeyItemPlaced(GameObject sender)
    {
        Debug.Log(sender.name + " placed!");

        // branching cases
        if (sender.name == firstBranchingObjA.name) {
            // branch A
            branch = 1;
            Debug.Log("Switching to branch A");
            // level complete
            if (firstKeyGameObjsA.Count == 0)
            {
                Debug.Log("First level puzzle complete branch A!");
                _videoControls.CompletePuzzle();

                level += 1;
                
                if (OnPuzzleComplete != null)
                {
                    OnPuzzleComplete();
                }
            } else {
                // _videoControls.ClipToPlay clip = _videoControls.ClipToPlay.BranchACorrupted;
                _videoControls.ChangeCorruptedVideo(ClipToPlay.BranchACorrupted);
            }
        } else if (sender.name == firstBranchingObjB.name) {
            // branch B
            branch = 2;
            Debug.Log("Switching to branch B");
            // level complete
            if (firstKeyGameObjsB.Count == 0)
            {
                Debug.Log("First level puzzle complete branch B!");
                _videoControls.CompletePuzzle();

                level += 1;
                
                if (OnPuzzleComplete != null)
                {
                    OnPuzzleComplete();
                }
            } else {
                // VideoControls.ClipToPlay clip = VideoControls.ClipToPlay.BranchBCorrupted;
                _videoControls.ChangeCorruptedVideo(ClipToPlay.BranchBCorrupted);
            }
        } else {
            if (level == 1) {
                if (firstKeyGameObjsA.Contains(sender)) {
                    firstKeyGameObjsA.Remove(sender);

                    // level complete
                    if (firstKeyGameObjsA.Count == 0)
                    {
                        Debug.Log("First level puzzle complete branch A!");
                        _videoControls.CompletePuzzle();

                        level += 1;
                        
                        if (OnPuzzleComplete != null)
                        {
                            OnPuzzleComplete();
                        }
                    }
                } else if (firstKeyGameObjsB.Contains(sender)) {
                    firstKeyGameObjsB.Remove(sender);

                    // level complete
                    if (firstKeyGameObjsB.Count == 0)
                    {
                        Debug.Log("First level puzzle complete branch B!");
                        _videoControls.CompletePuzzle();

                        level += 1;
                        
                        if (OnPuzzleComplete != null)
                        {
                            OnPuzzleComplete();
                        }
                    }
                }
            }
        }

        // keyItems.Remove(sender.GetComponent<ObjectDistance>());

    }
}
