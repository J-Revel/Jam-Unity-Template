using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
#endif

[System.Serializable]
public class WidgetDisplayState
{
    public Sprite sprite;
    public Color color = new Color(0, 0, 0, 0);

    public void Apply(SerializedProperty sprite_property, SerializedProperty color_property)
    {
        sprite_property.objectReferenceValue = sprite;
        color_property.colorValue = color;
    }
}

[ExecuteAlways, RequireComponent(typeof(FocusWidget))]
public class FocusButton: Image, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{

    [System.Serializable]
    public class ButtonDisplayStates
    {
        public WidgetDisplayState default_state;
        public WidgetDisplayState focus_state;
        public WidgetDisplayState pressed_state;
        public WidgetDisplayState disabled_state;
        public WidgetDisplayState disabled_focus_state;
    }
    
    private FocusWidget focus_widget;
    public ButtonDisplayStates display_states;
    public bool interactable = true;

    public UnityEvent on_click;
    public UnityEvent on_focus_gain;
    public UnityEvent on_focus_lost;
    
    private bool pointer_down = false;
    private FocusMenu menu;
    
    private void Awake()
    {
        menu = GetComponentInParent<FocusMenu>(true);
    }

    public void ApplyDisplay(WidgetDisplayState state, WidgetDisplayState default_state)
    {
        sprite = state.sprite == null ? default_state.sprite : state.sprite;
        color = state.color.a == 0 ? default_state.color: state.color;
    }

    void Start()
    {
        ApplyDisplay(display_states.default_state, display_states.default_state);
        focus_widget = GetComponentInParent<FocusWidget>();
        focus_widget.focus_gained_delegate += (stack) =>
        {
            ApplyDisplay(display_states.focus_state, display_states.default_state);
            on_focus_gain.Invoke();
        };
        focus_widget.focus_lost_delegate += (stack) =>
        {
            ApplyDisplay(display_states.default_state, display_states.default_state);
            on_focus_lost.Invoke();
        };
        focus_widget.confirm_delegate = () =>
        {
            on_click.Invoke();
            return true;
        };
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointer_down = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointer_down = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        on_click.Invoke();
    }

    public void Update()
    {
        if (Application.isPlaying)
        {
            if (pointer_down || (menu.confirm_action_down && focus_widget.has_focus))
            {
                ApplyDisplay(display_states.pressed_state, display_states.default_state);
            }
            else if (focus_widget.has_focus)
            {
                ApplyDisplay(display_states.focus_state, display_states.default_state);
            }
            else
            {
                ApplyDisplay(display_states.default_state, display_states.default_state);
            }
        }
    }
}

#if UNITY_EDITOR
    [CustomEditor(typeof(FocusButton))]
    [CanEditMultipleObjects]
    public class FocusButtonEditor : Editor
    {
        private int display_state;
        private Image target_image;

        private SerializedProperty sprite_prop, color_prop, display_states_prop, click_event_prop;
        void OnEnable()
        {
            target_image = (target as FocusButton).GetComponent<Image>();
            color_prop = serializedObject.FindProperty("m_Color");
            sprite_prop = serializedObject.FindProperty("m_Sprite");
            display_states_prop = serializedObject.FindProperty("display_states");
            click_event_prop = serializedObject.FindProperty("on_click");
            
            
            UpdateDisplay();
        }

        void OnDisable()
        {
            FocusButton button = (target as FocusButton);
            display_state = 0;
            UpdateDisplay();
        }

        void UpdateDisplay()
        {
            FocusButton button = (target as FocusButton);
            switch (display_state)
            {
                case 0:
                    button.display_states.default_state.Apply(sprite_prop, color_prop);
                    break;
                case 1:
                    button.display_states.focus_state.Apply(sprite_prop, color_prop);
                    break;
                case 2:
                    button.display_states.pressed_state.Apply(sprite_prop, color_prop);
                    break;
            }
        }
    
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            string button_text = "";
            switch (display_state)
            {
                case 0:
                    button_text = "Default State";
                    break;
                case 1:
                    button_text = "Focused State";
                    break;
                case 2:
                    button_text = "Pressed State";
                    break;
            }
            if (GUILayout.Button(button_text))
            {
                display_state = (display_state + 1) % 3;
                UpdateDisplay();
            }

            EditorGUILayout.PropertyField(display_states_prop);
            EditorGUILayout.PropertyField(click_event_prop);

            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
