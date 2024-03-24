using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class InteractionCue : ControlCue
{
    public List<InteractionType> interactions;


    protected override void SetActionHint(ActionHint actionHint)
    {
        if (!interactions.Contains(InteractableDetector.interactionType))
        {
            Debug.Log("Interaction type not found in list of interactions for this cue.");
            ResetCue();
            return;
        }

        Debug.Log("Setting action hint for interaction type: " + InteractableDetector.interactionType);
        currentAction = actionHint.action;
        text.text = GetInteractionText();
        UpdateCueSprite();
    }

    public static string ToFormattedText(InteractionType value)
    {
        return new string(value.ToString()
            .SelectMany(c =>
                char.IsUpper(c)
                ? new[] { ' ', c }
                : new[] { c })
            .ToArray()).Trim();
    }

    String GetInteractionText()
    {
        InteractionType interactionType = InteractableDetector.interactionType;
        return ToFormattedText(interactionType);
    }
}