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

    private float timeLeft;
    private bool noCue;
    [SerializeField] private float cueTime;

    [SerializeField] private List<GameObject> firstKeyGameObjsA;
    [SerializeField] private List<GameObject> firstKeyGameObjsB;

    [SerializeField] private GameObject firstBranchingObjA;
    [SerializeField] private GameObject firstBranchingObjB;

    public delegate void PuzzleComplete();
    public event PuzzleComplete OnPuzzleComplete;

    private VideoControls _videoControls;

    void Start()
    {
        _videoControls = FindObjectOfType<VideoControls>();
        level = 1;
        branch = 0;
        timeLeft = cueTime;
        noCue = true;
    }

    private void Awake()
    {
        firstBranchingObjA.GetComponent<ObjectDistance>().OnKeyItemPlaced += HandleKeyItemPlaced;
        firstBranchingObjB.GetComponent<ObjectDistance>().OnKeyItemPlaced += HandleKeyItemPlaced;

        foreach (var obj in firstKeyGameObjsA)
        {
            ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
            objDist.OnKeyItemPlaced += HandleKeyItemPlaced;
            objDist.enabled = false;
        }

        foreach (var obj in firstKeyGameObjsB)
        {
            ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
            objDist.OnKeyItemPlaced += HandleKeyItemPlaced;
            objDist.enabled = false;
        }
    }

    private void HandleKeyItemPlaced(GameObject sender)
    {
        Debug.Log(sender.name + " placed!");

        noCue = true;
        timeLeft = cueTime;

        // branching cases
        if (sender.name == firstBranchingObjA.name) {
            // branch A
            branch = 1;
            Debug.Log("Switching to branch A");
            // level complete
            if (firstKeyGameObjsA.Count == 0)
            {
                Debug.Log("First level puzzle complete branch A!");
                _videoControls.CompletePuzzle(ClipToPlay.BranchASolution);

                level += 1;
                
                if (OnPuzzleComplete != null)
                {
                    OnPuzzleComplete();
                }
            } else {
                ObjectDistance objDistA = firstBranchingObjA.GetComponent<ObjectDistance>();
                objDistA.targetObj.transform.GetChild(0).gameObject.SetActive(false);
                foreach (var obj in firstKeyGameObjsB)
                {
                    ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
                    objDist.targetObj.SetActive(false);
                }
                foreach (var obj in firstKeyGameObjsA)
                {
                    ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
                    objDist.enabled = true;
                    // Switch guitar target object
                    if (obj.name == "Guitar Model")
                    {
                        GameObject guitar = obj.transform.parent.gameObject;
                        GameObject guitarTarget = guitar.transform.GetChild(1).gameObject;
                        objDist.targetObj = guitarTarget;
                        objDist.targetObj.SetActive(false);
                    }
                }
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
                _videoControls.CompletePuzzle(ClipToPlay.BranchBSolution);

                level += 1;
                
                if (OnPuzzleComplete != null)
                {
                    OnPuzzleComplete();
                }
            } else {
                ObjectDistance objDistA = firstBranchingObjA.GetComponent<ObjectDistance>();
                objDistA.targetObj.transform.GetChild(0).gameObject.SetActive(false);
                foreach (var obj in firstKeyGameObjsA)
                {
                    ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
                    objDist.targetObj.SetActive(false);
                }
                foreach (var obj in firstKeyGameObjsB)
                {
                    ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
                    objDist.enabled = true;
                    // Switch guitar target object
                    if (obj.name == "Guitar Model")
                    {
                        GameObject guitar = obj.transform.parent.gameObject;
                        GameObject guitarTarget = guitar.transform.GetChild(2).gameObject;
                        objDist.targetObj = guitarTarget;
                        objDist.targetObj.SetActive(false);
                    }
                }
                _videoControls.ChangeCorruptedVideo(ClipToPlay.BranchBCorrupted);
            }
        } else {
            if (level == 1) {
                if (branch == 1 && firstKeyGameObjsA.Contains(sender)) {
                    firstKeyGameObjsA.Remove(sender);

                    // level complete
                    if (firstKeyGameObjsA.Count == 0)
                    {
                        Debug.Log("First level puzzle complete branch A!");
                        _videoControls.CompletePuzzle(ClipToPlay.BranchASolution);

                        level += 1;
                        
                        if (OnPuzzleComplete != null)
                        {
                            OnPuzzleComplete();
                        }
                    }
                } else if (branch == 2 && firstKeyGameObjsB.Contains(sender)) {
                    firstKeyGameObjsB.Remove(sender);

                    // level complete
                    if (firstKeyGameObjsB.Count == 0)
                    {
                        Debug.Log("First level puzzle complete branch B!");
                        _videoControls.CompletePuzzle(ClipToPlay.BranchBSolution);

                        level += 1;
                        
                        if (OnPuzzleComplete != null)
                        {
                            OnPuzzleComplete();
                        }
                    }
                }
            }
        }

    }

    void Update()
    {
        if (noCue) {
            timeLeft -= Time.deltaTime;
        }
        if ( timeLeft < 0 ) {
            // show cue

            if (branch == 0)
            {
                ObjectDistance objDistA = firstBranchingObjA.GetComponent<ObjectDistance>();
                objDistA.targetObj.transform.GetChild(0).gameObject.SetActive(true);
            } else if (branch == 1)
            {
                foreach (var obj in firstKeyGameObjsA)
                {
                    ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
                    objDist.targetObj.SetActive(true);
                }
            } else if (branch == 2)
            {
                foreach (var obj in firstKeyGameObjsB)
                {
                    ObjectDistance objDist = obj.GetComponent<ObjectDistance>();
                    objDist.targetObj.SetActive(true);
                }
            }
            noCue = false;
        }
    }
}
