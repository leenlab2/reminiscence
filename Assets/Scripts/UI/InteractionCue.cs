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
    public string text;
}

[Serializable]
public class InteractionCue : MonoBehaviour
{
    private Image image;
    private TMP_Text text;

    [Tooltip("The actions whose cues are displayed in this part of the screen.")]
    [SerializeField] private List<ActionHint> actionHints;

    private ActionHint currentActionHint;

    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        text = GetComponentInChildren<TMP_Text>();

        InteractionCueManager.OnControllerChanged += UpdateCueSprite;
    }

    void Start()
    {
        currentActionHint = actionHints[0];
        text.text = currentActionHint.text;
    }

    private void UpdateCueSprite()
    {
        if (currentActionHint.actionRef != null)
        {
            InputAction action = currentActionHint.actionRef.action;
            int index = action.GetBindingIndex(InteractionCueManager.bindingMask);
            string controlPath = action.bindings[index].effectivePath;
            Debug.Log(controlPath);
            image.sprite = InteractionCueManager.currentIcons.GetSprite(controlPath);
        }
    }

    private void SetActionHint(ActionHint actionHint)
    {
        currentActionHint = actionHint;
        text.text = currentActionHint.text;
        UpdateCueSprite();
    }

    private void Update()
    {
        foreach (ActionHint actionHint in actionHints)
        {
            if (actionHint.actionRef.action.enabled)
            {
                SetActionHint(actionHint);
                break;
            }
        }
    }
}
