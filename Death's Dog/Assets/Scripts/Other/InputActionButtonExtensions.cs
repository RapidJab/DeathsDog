using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public static class InputActionButtonExtensions
{
    public static bool GetButton(this InputAction action) => action == null ? false : action.ReadValue<float>() > 0;
    public static bool GetButtonDown(this InputAction action) => action == null ? false : action.triggered && action.ReadValue<float>() > 0;
    public static bool GetButtonUp(this InputAction action) => action == null ? false : action.triggered && action.ReadValue<float>() == 0;
}
