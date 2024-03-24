using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using UnityEditor;


public class InteractionCueManager : MonoBehaviour 
{
    public GamepadIcons xbox;
    public GamepadIcons ps4;
    public KeyboardIcons keyboard;

    private InputDevice currentDevice;
    public static IControlIcons currentIcons;
    public static String bindingMask = "Keyboard&Mouse";

    public static event Action OnControllerChanged;

    private void Start()
    {
        InputSystem.onActionChange += OnControllerChange;
    }

    #region Device Changes
    private void OnControllerChange(object obj, InputActionChange change)
    {
        if (obj != null && obj is InputAction action)
        {
            if (action.activeControl == null) return;
            InputDevice lastDevice = action.activeControl.device;

            if (lastDevice != currentDevice)
            {
                currentDevice = lastDevice;
                UpdateBindingMask();
                UpdateControlIcons();

                OnControllerChanged?.Invoke();
            }
        }
    }

    private void UpdateBindingMask()
    {
        bindingMask = currentDevice is Gamepad ? "Gamepad" : "Keyboard&Mouse";
    }

    private void UpdateControlIcons()
    {
        switch (currentDevice)
        {
            case XInputController:
                currentIcons = xbox;
                break;  
            case DualShockGamepad:
                currentIcons = ps4;
                break;
            default:
                currentIcons = keyboard;
                break;
        }
    }
    #endregion
}