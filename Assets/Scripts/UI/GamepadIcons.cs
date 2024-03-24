using System;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class GamepadIcons : IControlIcons
{
    public Sprite buttonSouth;
    public Sprite buttonNorth;
    public Sprite buttonEast;
    public Sprite buttonWest;
    public Sprite startButton;
    public Sprite leftTrigger;
    public Sprite rightTrigger;
    public Sprite leftShoulder;
    public Sprite rightShoulder;
    public Sprite leftStick;
    public Sprite rightStick;

    public Sprite GetSprite(string controlPath)
    {
        Debug.Log("getting sprite");
        // From the input system, we get the path of the control on device. So we can just
        // map from that to the sprites we have for gamepads.
        switch (controlPath)
        {
            case "<Gamepad>/buttonSouth": return buttonSouth;
            case "<Gamepad>/buttonNorth": return buttonNorth;
            case "<Gamepad>/buttonEast": return buttonEast;
            case "<Gamepad>/buttonWest": return buttonWest;
            case "<Gamepad>/start": return startButton;
            case "<Gamepad>/leftTrigger": return leftTrigger;
            case "<Gamepad>/rightTrigger": return rightTrigger;
            case "<Gamepad>/leftShoulder": return leftShoulder;
            case "<Gamepad>/rightShoulder": return rightShoulder;
            case "<Gamepad>/leftStick": return leftStick;
            case "<Gamepad>/rightStick": return rightStick;
        }
        Debug.Log("Coudlnt find asprite");
        return null;
    }
}