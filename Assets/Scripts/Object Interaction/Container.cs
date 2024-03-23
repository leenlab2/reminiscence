using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Interactable
{
    private Animator animator;
    public bool isOpen = false;

    public Transform attachPoint;
    public List<PickupInteractable> startingContents = new List<PickupInteractable>();

    void Awake()
    {
        base.Awake();
        animator = GetComponentInParent<Animator>();
    }

    private void Start()
    {
        // for gameobject in contents, move object to container
        foreach (PickupInteractable interactable in startingContents)
        {
            interactable.transform.SetParent(attachPoint);
        }
    }

    public void ToggleOpen()
    {
        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
        audioSource.Play();
    }
}
