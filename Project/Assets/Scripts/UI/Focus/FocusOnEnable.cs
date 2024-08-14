using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FocusWidget))]
public class FocusOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponentInParent<FocusMenu>().ForceFocus(GetComponent<FocusWidget>());
    }
}
