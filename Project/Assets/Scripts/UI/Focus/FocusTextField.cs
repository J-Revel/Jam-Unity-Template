using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(FocusButton))]
public class FocusTextField : MonoBehaviour
{
    public TMPro.TMP_InputField text_field;
    private FocusButton button;
    private bool focused = false;

    public void Start()
    {
        button = GetComponent<FocusButton>();
        button.on_focus_gain.AddListener(() =>
        {
            EventSystem.current.SetSelectedGameObject(text_field.gameObject);
        });
        button.on_focus_lost.AddListener(() =>
        {
            if(text_field.gameObject == EventSystem.current.currentSelectedGameObject)
                EventSystem.current.SetSelectedGameObject(null);
        });
    }
}
