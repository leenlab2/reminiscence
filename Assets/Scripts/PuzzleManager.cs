using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    private List<ObjectDistance> keyItems = new List<ObjectDistance>();

    public delegate void PuzzleComplete();
    public event PuzzleComplete OnPuzzleComplete;

    private void Awake()
    {
        keyItems = FindObjectsOfType<ObjectDistance>().ToList();
        foreach (var item in keyItems)
        {
            item.OnKeyItemPlaced += HandleKeyItemPlaced;
        }
    }

    private void HandleKeyItemPlaced(GameObject sender)
    {
        Debug.Log(sender.name + " placed!");

        keyItems.Remove(sender.GetComponent<ObjectDistance>());
        if (keyItems.Count == 0)
        {
            Debug.Log("Puzzle complete!");
            if (OnPuzzleComplete != null)
            {
                OnPuzzleComplete();
            }
        }
    }
}
