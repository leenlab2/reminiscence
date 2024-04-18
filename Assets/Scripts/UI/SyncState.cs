using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SyncState : MonoBehaviour
{
    public Selectable targetSelectable;

    [Header("Transition States")]
    public Image TargetGraphic;
    public Sprite SelectedSprite;
    private Sprite NormalSprite;

    // Start is called before the first frame update
    void Start()
    {
        // Make sure we have a reference to the target Selectable component
        if (targetSelectable == null)
        {
            Debug.LogError("Target Selectable not assigned!");
            return;
        }

        NormalSprite = TargetGraphic.sprite;
    }


    private void Update()
    {
        // Update the target graphic's sprite based on the target Selectable's state
        if (TargetGraphic.sprite != SelectedSprite &&
            targetSelectable == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>())
        {
            TargetGraphic.sprite = SelectedSprite;
        }
        else if (TargetGraphic.sprite == SelectedSprite &&
            targetSelectable != UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>())
        {
            TargetGraphic.sprite = NormalSprite;
        }
    }
}
