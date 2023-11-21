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

    private InteractableDetector detector;
    private Animator dialAnimator;
    public GameObject dialBaseModel;
    public GameObject dialStickModel;

    void OnEnable()
    {
        dialBaseModel.SetActive(false);
        dialStickModel.SetActive(false);

        inText.color = Color.white;
        outText.color = Color.yellow;
    }

    void OnDisable()
    {
        dialBaseModel.SetActive(true);
        dialStickModel.SetActive(true);
    }

    void Start()
    {
        detector = FindAnyObjectByType<InteractableDetector>();
        dialAnimator = dial.GetComponent<Animator>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("Selected");
        if (eventData.selectedObject == gameObject)
        {
            Debug.Log("button selected");
            detector.highlightObject(dial);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (eventData.selectedObject == gameObject)
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
