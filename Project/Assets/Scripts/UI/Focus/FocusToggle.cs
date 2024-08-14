using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusToggle : Toggle
{
    public void Toggle()
    {
        isOn = !isOn;
    }
}
