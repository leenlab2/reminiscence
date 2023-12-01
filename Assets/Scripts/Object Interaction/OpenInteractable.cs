using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Outline))]
public class OpenInteractable : MonoBehaviour
{
    [Header("Sound effects")]
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioSource audioSource;

    private Animator animator;
    private bool isOpen = false;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    public void ToggleOpen()
    {
        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
    }
}
