using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is needed for the AnimationEvent as an intermediary script
public class DialController : MonoBehaviour
{
    public void SwitchToIn()
    {
        GetComponentInParent<MemoryTransition>().SwitchToIn();
    }

    public void SwitchToOut()
    {
        GetComponentInParent<MemoryTransition>().SwitchToOut();
    }
}
