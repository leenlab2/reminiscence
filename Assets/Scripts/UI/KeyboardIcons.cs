using System;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class KeyboardIcons : IControlIcons
{
    public Sprite w;
    public Sprite a;
    public Sprite s;
    public Sprite d;
    public Sprite e;
    public Sprite t;
    public Sprite tab;
    public Sprite enter;
    public Sprite escape;
    public Sprite leftButton;
    public Sprite delta;

    public Sprite error;

    public Sprite GetSprite(string controlPath)
    {
        // From the input system, we get the path of the control on device. So we can just
        // map from that to the sprites we have for gamepads.
        switch (controlPath)
        {
            case "<Keyboard>/w": return w;
            case "<Keyboard>/a": return a;
            case "<Keyboard>/s": return s;
            case "<Keyboard>/d": return d;
            case "<Keyboard>/e": return e;
            case "<Keyboard>/t": return t;
            case "<Keyboard>/tab": return tab;
            case "<Keyboard>/enter": return enter;
            case "<Keyboard>/escape": return escape;
            case "<Mouse>/leftButton": return leftButton;
            case "<Pointer>/delta": return delta;
        }

        return error;
    }
}