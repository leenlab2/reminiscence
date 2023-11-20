using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MemoryTransition : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject dial;
    [SerializeField] private TextMeshProUGUI inText;
    [SerializeField] private TextMeshProUGUI outText;
    [SerializeField] private SceneManagement sceneManagement;
    [SerializeField] private Button uiButton;

    private InteractableDetector detector;
    private Animator dialAnimator;

    void Start()
    {
        detector = FindAnyObjectByType<InteractableDetector>();
        dialAnimator = dial.GetComponent<Animator>();

        inText.color = Color.white;
        outText.color = Color.yellow;
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("Selected");
        if (eventData.selectedObject == uiButton.gameObject)
        {
            Debug.Log("button selected");
            detector.highlightObject(dial);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (eventData.selectedObject == uiButton.gameObject)
        {
            detector.unhighlightObject(dial);
        }
    }

    public void SwitchToIn()
    {
        inText.color = Color.yellow;
        outText.color = Color.white;

        sceneManagement.EnterMemoryScene();
        dialAnimator.SetTrigger("Move");
    }

    public void SwitchToOut()
    {
        inText.color = Color.white;
        outText.color = Color.yellow;
    }
}
