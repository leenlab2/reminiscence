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
    public InputActionReference actionRef;
    public InputAction action => ControlCue.GetActionFromRef(actionRef);
    public string text;
}

public class ControlCue : MonoBehaviour
{
    protected Image image;
    protected TMP_Text text;

    [Tooltip("The actions whose cues are displayed in this part of the screen.")]
    [SerializeField] protected List<ActionHint> actionHints;

    protected InputAction currentAction = null;

    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        text = GetComponentInChildren<TMP_Text>();

        ControlCueManager.OnControllerChanged += UpdateCueSprite;
    }

    private void OnDestroy()
    {
        ControlCueManager.OnControllerChanged -= UpdateCueSprite;
    }

    public static InputAction GetActionFromRef(InputActionReference actionRef)
    {
        InputActionMap actionMap = InputManager.instance.playerInputActions.asset.FindActionMap(actionRef.action.actionMap.name);
        if (actionMap == null) return null;


        return actionMap.FindAction(actionRef.action.name);
    }

    protected void UpdateCueSprite()
    {
        if (currentAction != null)
        {
            int index = currentAction.GetBindingIndex(ControlCueManager.bindingMask);
            string controlPath = currentAction.bindings[index].effectivePath;
            Sprite mysprite = ControlCueManager.currentIcons.GetSprite(controlPath);
            image.sprite = mysprite;
        }
    }

    protected virtual void SetActionHint(ActionHint actionHint)
    {
        currentAction = actionHint.action;
        text.text = actionHint.text;
        UpdateCueSprite();
    }

    protected void ResetCue()
    {
        currentAction = null;
        image.sprite = null;
        text.text = "";
    }

    protected virtual void Update()
    {
        if (currentAction != null && currentAction.enabled && GetComponent<InteractionCue>() == null) return;

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
        } else if (currentAction != null && !currentAction.enabled)
        {
            ResetCue();
        }
    }
}
