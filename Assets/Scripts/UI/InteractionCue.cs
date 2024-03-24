using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


[Serializable]
public struct ActionHint
{
    public String actionPath;
    public InputAction action => InputManager.instance.playerInputActions.FindAction(actionPath);
    public string text;
}

[Serializable]
public class InteractionCue : MonoBehaviour
{
    private Image image;
    private TMP_Text text;

    [Tooltip("The actions whose cues are displayed in this part of the screen.")]
    [SerializeField] private List<ActionHint> actionHints;

    private InputAction currentAction;

    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        text = GetComponentInChildren<TMP_Text>();

        InteractionCueManager.OnControllerChanged += UpdateCueSprite;
    }

    private void UpdateCueSprite()
    {
        if (currentAction != null)
        {
            int index = currentAction.GetBindingIndex(InteractionCueManager.bindingMask);
            string controlPath = currentAction.bindings[index].effectivePath;
            Debug.Log(controlPath);
            image.sprite = InteractionCueManager.currentIcons.GetSprite(controlPath);
        }
    }

    private void SetActionHint(ActionHint actionHint)
    {
        currentAction = actionHint.action;
        text.text = actionHint.text;
        UpdateCueSprite();
    }

    void ResetCue()
    {
        currentAction = null;
        image.sprite = null;
        text.text = "";
    }

    void Update()
    {
        ActionHint? newActionHint = null;
        foreach (ActionHint actionHint in actionHints)
        {
            if (actionHint.action.enabled)
            {
                newActionHint = actionHint;
                break;
            }
        }

        if (newActionHint != null)
        {
            SetActionHint((ActionHint)newActionHint);
        } else
        {
            ResetCue();
        }
    }
}
