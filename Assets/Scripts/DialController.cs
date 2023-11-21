using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is needed for the AnimationEvent as an intermediary script
public class DialController : MonoBehaviour
{
    public GameObject dialBaseModel;
    public GameObject dialStickModel;

    private void OnEnable()
    {
        dialBaseModel.SetActive(false);
        dialStickModel.SetActive(false);
    }

    private void OnDisable()
    {
        dialBaseModel.SetActive(true);
        dialStickModel.SetActive(true);
    }

    public void SwitchToIn()
    {
        GetComponentInParent<UISelector_3D>().SwitchToIn();
    }

    public void SwitchToOut()
    {
        GetComponentInParent<UISelector_3D>().SwitchToOut();
    }
}
