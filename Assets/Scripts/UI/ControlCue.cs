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
    public InputAction action => InputManager.instance.playerInputActions.FindAction(actionRef.action.name);
    public string text;
}


public class ControlCue : MonoBehaviour
{
    protected Image image;
    protected TMP_Text text;

    [Tooltip("The actions whose cues are displayed in this part of the screen.")]
    [SerializeField] protected List<ActionHint> actionHints;

    protected InputAction currentAction;

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

    protected void UpdateCueSprite()
    {
        if (currentAction != null)
        {
            int index = currentAction.GetBindingIndex(ControlCueManager.bindingMask);
            string controlPath = currentAction.bindings[index].effectivePath;
            IControlIcons myicons = ControlCueManager.currentIcons;
            Sprite mysprite = myicons.GetSprite(controlPath);
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
        } else if (GetComponent<InteractionCue>() == null)
        {
            ResetCue();
        }
    }
}
