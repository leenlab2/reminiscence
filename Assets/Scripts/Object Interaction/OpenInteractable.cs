using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenInteractable : Interactable
{
    private Animator animator;
    public bool isOpen = false;

    void Awake()
    {
        base.Awake();
        animator = GetComponentInParent<Animator>();
    }

    public void ToggleOpen()
    {
        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
    }
}
