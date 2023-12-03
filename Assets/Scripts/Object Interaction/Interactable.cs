using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Outline))]
public class Interactable : MonoBehaviour
{
    [Header("Object Interaction")]
    [SerializeField] public bool isInteractable = false;

    [Header("Sound effects")]
    [SerializeField] private AudioClip enterSound;
    [SerializeField] private AudioClip exitSound;
    [SerializeField] private AudioSource audioSource;

    protected void Awake()
    {
        TapeManager.OnFirstTapeInserted += EnableInteraction;
    }

    protected void OnDestroy()
    {
        TapeManager.OnFirstTapeInserted -= EnableInteraction;
    }

    protected void EnableInteraction()
    {
        isInteractable = true;
    }
}
