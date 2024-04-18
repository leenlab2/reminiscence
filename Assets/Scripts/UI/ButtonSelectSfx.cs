using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectSfx : MonoBehaviour, ISelectHandler

{
    public AudioSource source;
    public AudioClip sfx;

    public void OnSelect(BaseEventData eventData)
    {
        source.PlayOneShot(sfx);
    }
}
